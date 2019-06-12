from random import *
from Storage import *

class TheClass:
    Store = VarStorage()

    MovieList = []
    IterInt = 0
    RandInt = 0
    ListName = ""
    NumberOptions = int()
    def MakeMovieList(self):
        while TheClass.IterInt < TheClass.NumberOptions:
            TheClass.MovieList.append(input("Movie name?\n"))
            TheClass.IterInt += 1
        TheClass.IterInt = 0

    def RandomInt(self):
        TheClass.RandInt = randint(0, TheClass.NumberOptions - 1)
        print("The movie is: " + TheClass.MovieList[TheClass.RandInt])

    def GetList(self):
        GetListName = TheClass.Store.ListStore.keys()
        ListNamesStr = str(GetListName)
        GetConfirm = int(input(ListNamesStr + " using numbers, which list?")) - 1
        ## Need to get the movie list

    def EndFunction(self):
        RemoveOption = TheClass.MovieList[TheClass.RandInt]
        TheClass.MovieList.remove(RemoveOption)
        VarStorage.ListStore[TheClass.ListName] = TheClass.MovieList
        # Write to storage
        TheClass.MovieList.clear()

    def FileWorks(self, x):

        pass
