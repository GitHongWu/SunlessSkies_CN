import json
import tempfile
import unittest
import zipfile
from pathlib import Path
from branch_priority import resolve

class BranchPriorityTests(unittest.TestCase):
    def test_priority_context_conflict_and_invalid(self):
        def row(key, value):
            return dict(key=key, original=json.dumps({'Reminder': {'1': 'Same'}}),
                        translation=json.dumps({'Reminder': {'1': value}}))
        with tempfile.TemporaryDirectory() as temp:
            snapshot = Path(temp) / 'snapshot.zip'
            with zipfile.ZipFile(snapshot, 'w') as archive:
                archive.writestr('utf8/2.0.7.0/qualities.json', json.dumps([row('1$VariableDescriptionText', 'preferred')]))
                archive.writestr('utf8/NEW/qualities.json', json.dumps([
                    row('1$VariableDescriptionText', 'new'),
                    row('2$VariableDescriptionText', 'context A'),
                    row('2$VariableDescriptionText', 'context A'),
                    dict(key='broken', original='{', translation='{')]))
            selected, report = resolve(snapshot, [(json.dumps({'1':'Same'}), json.dumps({'1':'legacy'}))])
            self.assertEqual(selected['Same'], 'preferred')
            self.assertEqual(report['contexts']['2(Reminder)\tSame'], 'context A')
            self.assertTrue(report['invalid'])
            override=row('3$VariableDescriptionText','equal rank conflict');override['file']='2.0.7.0/qualities.json'
            selected, report=resolve(snapshot, [], [override])
            self.assertNotIn('Same',selected)
            self.assertIn('Same',report['conflicts'])
            self.assertEqual(report['contexts']['1(Reminder)\tSame'],'preferred')

if __name__ == '__main__':
    unittest.main()
