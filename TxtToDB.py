import os
import sqlite3
import random as ran


def process_txt(route):
    new_movie = open(route, "r")
    linebreak = "\n"

    mov_arr = new_movie.readlines()
    final_arr = []

    for raw_mov in mov_arr:
        raw_mov = raw_mov.removesuffix(linebreak)
        final_arr.append(raw_mov)

    return final_arr


if __name__ == '__main__':
    testID = 0
    new_conn = sqlite3.connect("SaveFile.db")
    data_cursor = new_conn.cursor()

    route_one = "ResourceFiles/BadMoviesListOld.txt"
    route_two = "ResourceFiles/MovieListFeed.txt"

    out_list = process_txt(route_one)

    for entry in out_list:
        ran.seed()
        rank = out_list.index(entry) + 1
        new_id = ran.randint(0, 2147483647)
        new_id = int(new_id)
        entry_data = (new_id, 1, 1, "Movie not rejected", "Movie not reviewed", rank, rank, rank, rank, entry)
        data_cursor.execute("""INSERT INTO movies VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?)""", entry_data)
        new_conn.commit()


    print("\nend route one\n")

    out_list_two = process_txt(route_two)
    for entry in out_list_two:
        ran.seed()
        new_id = ran.randint(0, 2147483647)
        new_id = int(new_id)
        entry_data = (new_id, 0, 0, "Movie not rejected", "Movie not yet reviewed", 0, 0, 0, 0, entry)
        data_cursor.execute("""INSERT OR REPLACE INTO movies VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?)""", entry_data)
        new_conn.commit()

    #new_conn.commit()
    new_conn.close()


