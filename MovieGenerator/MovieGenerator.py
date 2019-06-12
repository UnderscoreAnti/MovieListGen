
#
# TODO: Actually save entries
# TODO: Find a way to get the dictionary key and entry to operate on.
# TODO: Get a list, assign it to a variable and perform the choice function
#
from TheClass import *
import Storage
def MakeList():
    Start = TheClass()
    TheClass.NumberOptions = int(input('How many movies in the list?\n'))
    TheClass.ListName = input("And name the list\n")
    Start.MakeMovieList()
    Start.RandomInt()
    Start.EndFunction()

def GetList():
    Start = TheClass()
    Start.GetList()

def ProgramActive():
    Quit = 0
    Command = input("Make list (1), get list (2), or quit (3)?\n")
    if(Command == '1' or Command == 'make list'):
        MakeList()
    elif(Command == '2' or Command == 'get list'):
        GetList()
    elif(Command == '3' or Command == 'quit'):
        Quit = 1
    else:
        print("Try again please, use the numbers")
    if(Quit == 0):
        ProgramActive()

ProgramActive()


