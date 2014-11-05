import os 
import re 

print os.getcwd()
sentence = 'A project with an Output Type of Class Library cannot be started directly'

print re.findall(r'\w+', sentence)
