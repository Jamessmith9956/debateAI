from youtube_transcript_api import YouTubeTranscriptApi
from youtube_transcript_api.formatters import JSONFormatter


def get_transcript(id):
    formatter = JSONFormatter()
    data = YouTubeTranscriptApi.get_transcript(id)
    data = combine_overlapping(data)
    data = formatter.format_transcript(data)
    return data



def combine_overlapping(transcript):
    i=0
    while i+1<len(transcript):
        if transcript[i+1]['start'] < transcript[i]['start'] + transcript[i]['duration']:
            transcript[i]['duration'] = transcript[i+1]['start'] + transcript[i+1]['duration'] - transcript[i]['start']
            transcript[i]['text'] += ' ' + transcript[i+1]['text']
            transcript.pop(i+1)
        else:
            i += 1
    return transcript

def clean(transcript):
    return transcript