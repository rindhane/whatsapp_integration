import matplotlib.pyplot as plt
import sys
import json

#get args:
file_path=sys.argv[1]
DataString=sys.argv[2]
Npairs=sys.argv[3]
#checked received DataString
print(DataString)
#function to convert data string in args into python dictionary
def get_dict_from_string(string):
    #string structure is : "col1:val1;col2:val2;col3:val3;col4:val4;....."
    col_val_pairs=string.split(';')
    result=dict()
    for pair in col_val_pairs:
        col_val_pair=pair.split(":")
        result[col_val_pair[0]]=int(col_val_pair[1])
    return result    

#load & construct data from args
Data=get_dict_from_string(DataString)
blocks=list(Data.keys())
values = [Data[i] for i in blocks ] 
#sample constructed  data
#blocks = ["A", "B", "C", "D"]
#values = [51,43,55,56]
print(f"pairs received are {len(Data.keys())} as compared to indicated no:{Npairs}")
#construct the chart 
# fig = plt.figure(figsize = (10, 5)) #set fig_size
color="maroon"
width=0.4
plt.bar(blocks,values, label="no.") 
#labelling: 
plt.xlabel("Production Blocks")
plt.ylabel("Current Output")
plt.title("Block wise Output")
#Add a legend
plt.legend()
# function to add value labels on individual bars
def addlabels(x,y):
    for i in range(len(x)):
        plt.text(i, y[i], y[i], ha = 'center')
addlabels(blocks, values)

#save the fig
plt.savefig(file_path)

#now build the script through pysinstaller with command :\> pyinstaller chartLib.py

#Ref:
    #https://www.geeksforgeeks.org/adding-value-labels-on-a-matplotlib-bar-chart/

#command:  python .\chartLib.py foo.jpg '{"key1":"val1", "key2":[val2, val3], "key3":"val4"}'