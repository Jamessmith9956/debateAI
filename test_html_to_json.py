import pytest
from html_to_json import parse_youtube_transcript, write_json, read_html, transcript_entry

class TestTranscript:
    def test_parse_youtube_transcript(self):
        # Test case 1: Empty data
        data = {"data": {"div": {"ytd-transcript-segment-renderer": []}}}
        expected_output = []
        assert parse_youtube_transcript(data) == expected_output

        # Test case 2: Single entry
        data = {
            "data": {
                "div": {
                    "ytd-transcript-segment-renderer": [
                        {
                            "div": {
                                "div": {"div": {"#text": "00:00:00.000"}},
                                "yt-formatted-string": {"#text":"Hello"}
                            }
                        }
                    ]
                }
            }
        }
        expected_output = [
            transcript_entry("00:00:00.000", "Hello")
        ]
        assert parse_youtube_transcript(data) == expected_output

        # Test case 3: Multiple entries
        data = {
            "data": {
                "div": {
                    "ytd-transcript-segment-renderer": [
                        {
                            "div": {
                                "div": {
                                    "div": {"#text": "00:00:00.000"}
                                },
                                "yt-formatted-string": {"#text":"Hello"}
                            }
                        },
                        {
                            "div": {
                                "div": {
                                    "div": {"#text": "00:01:30.500"}
                                },
                                "yt-formatted-string": {"#text":"World"}
                            }
                        }
                    ]
                }
            }
        }
        expected_output = [
            transcript_entry("00:00:00.000", "Hello"),
            transcript_entry("00:01:30.500", "World")
        ]
        assert parse_youtube_transcript(data) == expected_output

    def test_write_json(self):
        def clean(text):
            # replace with json handling if its not too compute expensive
            #strip whitespaces, newlines 
            return text.replace(" ","").replace("\n","")
        
        # Test case 1: Empty transcript
        transcript = []
        outfile = 'test/transcript.json'
        write_json(transcript, outfile)
        with open(outfile, 'r') as file:
            json_text = file.read()
            assert clean(json_text) == clean('{"data": []}')

        # Test case 2: Non-empty transcript
        transcript = [
            transcript_entry("00:00:00.000", "Hello"),
            transcript_entry("00:01:30.500", "World")
        ]
        outfile = 'test/transcript.json'
        write_json(transcript, outfile)
        with open(outfile, 'r') as file:
            json_text = file.read()
            #strip tab and newlines 
            json_text = json_text.replace("\t","").replace("\n","")
            assert clean(json_text) == clean('{"data": [{"timestamp": "00:00:00.000", "text": "Hello"}, {"timestamp": "00:01:30.500", "text": "World"}]}')

    def test_read_html(self):
        # Test case 1: Empty file
        filename = 'test/empty.html'
        with open(filename, 'w') as file:
            pass
        with pytest.raises(FileNotFoundError):
            read_html(filename)

        # Test case 2: Non-empty file
        filename = 'test/sample.html'
        with open(filename, 'w') as file:
            file.write('<div><ytd-transcript-segment-renderer><div><div><div>#text</div></div><yt-formatted-string>[&nbsp;__&nbsp;]#text&nbsp;&nbsp;</yt-formatted-string></div></ytd-transcript-segment-renderer></div>')
                       
        expected_output = {
            "div": {
                "ytd-transcript-segment-renderer": {
                    "div": {
                        "div": {
                            "div": "#text"
                        },
                        "yt-formatted-string": "#text"
                    }
                }
            }
        }
        assert read_html(filename) == expected_output

        # Test case 3: Input with executable code or escaped characters
        filename = 'test/executable.html'
        with open(filename, 'w') as file:
            file.write('<div><ytd-transcript-segment-renderer><div><div><div>#text</div></div></div><div><yt-formatted-string>print("Hello, World!")</yt-formatted-string></div></ytd-transcript-segment-renderer></div>')
        with pytest.raises(ValueError):
            read_html(filename)


if __name__ == '__main__':
    pytest.main()
