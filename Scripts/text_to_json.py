import re
import pandas as pd
#from card import card


def parse_api_transcript(text):
    pattern = r'(\d{1,2}:\d{2}:\d{2}\.\d{3}),(\d{1,2}:\d{2}:\d{2}\.\d{3})\n(.*)'
    matches = re.findall(pattern, text, re.MULTILINE)
    result = [{'id': i, 'start': match[0], 'end': match[1], 'text': match[2].strip()} for i, match in enumerate(matches)]
    #result = [card(start=match[0], end=match[1], text= match[2].strip()) for match in matches]
    #result = [card(card_id=i, start=match[0], end=match[1], text= match[2].strip()) for i, match in enumerate(matches)]
    combine_overlapping_cards(result)
    return result


def combine_overlapping_cards(cards):
    # sort the cards by start time
    cards.sort(key=lambda x: x['start'])
    # iterate through the cards and combine overlapping cards
    
    i = 0
    while i < len(cards) - 1:
        if cards[i]['end'] > cards[i+1]['start']:
            cards[i]['end'] = cards[i+1]['end']
            cards[i]['text'] += " " + cards[i+1]['text']
            del cards[i+1]
        else:
            i += 1
    return cards


with open("./tests/scripts.tests/samples/sample.txt", "r") as file:
    # read the file
    text = file.read()

cards = parse_api_transcript(text)