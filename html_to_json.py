import xmltojson
import json 

class transcript_entry:
    def __init__(self, timestamp, text):
        self.timestamp = timestamp
        self.text = text
    
    def __eq__(self, other):
        return self.timestamp == other.timestamp and self.text == other.text
        
###
  # Naively parse the html and extract the transcript
  #     In future search for the traget divs (saved in variables) and then parse the divs
###
def parse_youtube_transcript(data):
    # data > div > ytd-transcript-segment-renderer  
    transcript = []
    
    # exit early if data is empty
    if(len(data) == 0): return transcript 
    
    for object in data["data"]["div"]["ytd-transcript-segment-renderer"]:
        transcript.append(
            transcript_entry(
                object["div"]["div"]["div"]["#text"],
                object["div"]["yt-formatted-string"]["#text"]
            )
        )
    # convert to dict
    return transcript

def write_json(transcript, outfile):
    with open(outfile, 'w') as outfile:
        # convert the transcript to json
        json_text = {"data":[{"timestamp": entry.timestamp, "text": entry.text} for entry in transcript]}
        json.dump(json_text, outfile, indent=4)

def read_html(filename):
    with open(filename,"r") as file:
        html = file.read()
        # if the string is empty, raise a file not found error
        if(len(html) == 0): raise FileNotFoundError("File is empty")
        
        # replace string "[&nbsp;__&nbsp;]" with "" before parsing, consider pulling into a function
        html = html.replace("&nbsp;","")
        html = html.replace("[__]","")
        json_text = xmltojson.parse(html) # consider just using xmltodict directly  
        return json.loads(json_text)


def main():
    data = read_html("sample-.txt")
    transcript = parse_youtube_transcript(data)
    write_json(transcript)
    
