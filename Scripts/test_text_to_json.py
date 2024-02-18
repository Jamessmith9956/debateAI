import pytest
from text_to_json import parse_api_transcript

class TestTranscript:
    def test_parse_api_transcript(self):
        # Test case 1: Empty data
        text = ""
        expected_output = []
    
    def test_parse_api_transcript(self):
        # Test case 2: Single entry
        text = "00:00:00.000,00:00:00.000\nHello"
        expected_output = [
            {'timestamp': '00:00:00.000', 'text': 'Hello'}
        ]
    
    def test_parse_api_transcript(self):
        # Test case 3: Multiple entries
        text = "00:00:00.000,00:00:00.000\nHello\n00:01:30.500,00:01:30.500\nWorld"
        expected_output = [
            {'timestamp': '00:00:00.000', 'text': 'Hello'},
            {'timestamp': '00:01:30.500', 'text': 'World'}
        ]