import pandas as pd

data = pd.read_csv('C:\\Users\\Grady\\Desktop\\CIS556\\project\\unigram_freq.csv')
words = []

i = 0
while len(words) < 150000:
    try:
        if len(data['word'][i]) > 2:
            words.append(data['word'][i])
    except:
        pass
    i += 1

f = open("Dictionary.txt", "w")
for w in words:
    f.write(w+"\n")
f.close()
