import os
import sqlite3

if __name__ == '__main__':
    old_movie = open("ResourceFiles/BadMoviesListOld.txt", "r")
    new_movie = open("ResourceFiles/MovieListFeed.txt", "r")

    new_conn = sqlite3.connect("SaveFile.db")

    data_cursor = new_conn.cursor()

    result = data_cursor.execute("COMMAND command COMMMAND comm")
