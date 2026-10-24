"""Resolve nested translation branches by source, without guessing equal-rank conflicts.

Inputs are decoded (original, translation) pairs, as returned by XUnity's decoder.
Preserve runtime-only aliases/corrections by merging the returned unique choices
into the existing branch table; unresolved keys are reported, never overwritten.
"""
import json
import zipfile
from collections import defaultdict


def rank(path):
    return 0 if path.startswith('2.0.7.0/') else 1 if path.startswith('NEW/') else 2


def leaves(original, translation, path=()):
    if isinstance(original, dict) and isinstance(translation, dict):
        for key in sorted(original.keys() & translation.keys()):
            yield from leaves(original[key], translation[key], path + (key,))
    elif isinstance(original, str) and isinstance(translation, str) and original and translation.strip():
        yield original.replace('\r\n', '\n'), translation.replace('\r\n', '\n'), path


def resolve(snapshot, legacy_pairs, overrides=()):
    """Overrides are reviewed rows with file/key/original/translation, not a new priority tier."""
    with zipfile.ZipFile(snapshot) as archive:
        rows = [dict(row, file=name[5:]) for name in sorted(archive.namelist())
                if name.startswith('utf8/') and name.endswith('.json')
                for row in json.loads(archive.read(name))]
    by_identity = {(r['file'], r['key'], r['original']): r for r in rows}
    for row in overrides:
        by_identity[(row['file'], row['key'], row['original'])] = row
    choices = defaultdict(list)
    contextual = defaultdict(list)
    invalid = []

    def add(original, translation, priority, source):
        if not original.startswith('{') or not translation.startswith('{'):
            return
        try:
            pairs = list(leaves(json.loads(original), json.loads(translation)))
        except (ValueError, TypeError) as error:
            invalid.append(dict(source=source, error=str(error)))
            return
        for key, value, path in pairs:
            candidate = dict(translation=value, priority=priority, source=source, branch=list(path))
            choices[key].append(candidate)
            if source.endswith('$VariableDescriptionText') and len(path) == 2:
                quality = source.rsplit(':', 1)[1].split('$', 1)[0]
                contextual[quality + '(' + path[0] + ')\t' + key].append(candidate)
            elif len(path) == 1 and source.endswith(('$LevelDescriptionText', '$ChangeDescriptionText', '$DescendingChangeDescriptionText')):
                contextual[source.rsplit(':', 1)[1] + '\t' + key].append(candidate)

    for row in by_identity.values():
        add(row['original'], row.get('translation') or '', rank(row['file']),
            row['file'] + ':' + row['key'])
    for original, translation in legacy_pairs:
        add(original, translation, 3, 'existing dictionary fallback')
    selected, conflicts, resolved = {}, {}, {}
    for key, candidates in sorted(choices.items()):
        best_rank = min(c['priority'] for c in candidates)
        best = {c['translation'] for c in candidates if c['priority'] == best_rank}
        if len(best) != 1:
            conflicts[key] = candidates
            continue
        selected[key] = next(iter(best))
        if len({c['translation'] for c in candidates}) > 1:
            resolved[key] = dict(translation=selected[key], candidates=candidates)
    contexts, context_conflicts = {}, {}
    for key, candidates in sorted(contextual.items()):
        priority = min(c['priority'] for c in candidates)
        best = {c['translation'] for c in candidates if c['priority'] == priority}
        if len(best) != 1:
            context_conflicts[key] = candidates
        elif selected.get(key.split('\t', 1)[1]) != next(iter(best)):
            contexts[key] = next(iter(best))
    return selected, dict(resolved=resolved, conflicts=conflicts, invalid=invalid,
                          contexts=contexts, context_conflicts=context_conflicts)
