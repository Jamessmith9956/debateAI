# import basic word tokenizer to count the number of words in a sentence
import matplotlib.pyplot as plt
from matplotlib.ticker import FuncFormatter
from itertools import cycle
import pandas as pd
import string


# 
def count_words_string(sentence):
    num_words = sum([i.strip(string.punctuation).isalpha() for i in sentence.split()])
    return num_words

# find the differnece in time between two timestamps
###
#  # time_diff
#  #   start: string
#  #   end: string
#  #   returns: datetime
###
def time_diff(start, end) -> pd.Timedelta:
    return pd.to_timedelta(end) - pd.to_timedelta(start)

# np.lib.stride_tricks.sliding_window_view could be used, but it has O(N*W) complexity, vs below O(N) sol
# Behavious is undefined for the last sentence since I haven't captured the duration of each piece of text
# this analysis doesn't work as some timestamps overlap. I need to take in both the start and the end of the timestamp,
#   or average over a longer period of time.
def wpm(cards): 
    wpm = []
    card = cycle(cards)
    next_card = next(card)
    for i in range(len(cards)-1):
        this_card, next_card = next_card, next(card)
        diff = time_diff(this_card['start'], next_card['start'])
        count = count_words_string(this_card['text'])
        wpm.append([count/diff.seconds*60,diff.seconds])
    return wpm
        
# Plotting tools

def format_func(x, pos):
    minutes = int(x//60)
    seconds = int(x%60)
    return "{:d}:{:02d}".format(minutes, seconds)

def plot_wpm(wpm):
    # convert 2nd column from delta to total seconds so far
    total = 0
    for w in wpm:
        total += w[1]
        w[1] = total
    # plot the wpm over time, where the 2nd column is the time in seconds
    wpm = pd.DataFrame(wpm, columns = ['wpm', 'time'])
    
    formatter = FuncFormatter(format_func)
    f = plt.figure()
    ax = f.add_subplot(1,1,1)
    # create a line graph of the wpm over time
    ax.plot(wpm['time'], wpm['wpm'])
    
    ax.xaxis.set_major_formatter(formatter)
    # locate the tick marks on the minute
    #ax.xaxis.set_major_locator(plt.MultipleLocator(60))
    ax.xaxis.set_major_locator(plt.MultipleLocator(base=30))
    
    ax.set_title("WPM over time")
    ax.set_xlabel("Time (minutes:seconds)")
    ax.set_ylabel("Words per minute")
    
    #plot and save
    plt.savefig("wpm.png")


    