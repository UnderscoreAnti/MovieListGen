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


def create_id(check_arr):
    ran.seed()
    new_id = ran.randint(0, 2147483647)
    new_id = int(new_id)

    while new_id in check_arr:
        new_id = ran.randint(0, 2147483647)
        new_id = int(new_id)

    return new_id


if __name__ == '__main__':
    id_arr = []
    new_conn = sqlite3.connect("SaveFile.db")
    data_cursor = new_conn.cursor()

    route_one = "ResourceFiles/BadMoviesListOld.txt"
    route_two = "ResourceFiles/MovieListFeed.txt"

    data_cursor.execute("""DELETE FROM movies""")
    print("End route -1")

    out_list = process_txt(route_one)

    for entry in out_list:
        rank = out_list.index(entry) + 1
        mov_id = create_id(id_arr)
        id_arr.append(mov_id)
        entry_data = (mov_id, 1, 1, "Movie not rejected", "Movie not reviewed", rank, rank, rank, rank, entry)
        data_cursor.execute("""INSERT INTO movies VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?)""", entry_data)
        new_conn.commit()


    print("\nend route one\n")

    out_list_two = process_txt(route_two)

    for entry in out_list_two:
        mov_id = create_id(id_arr)
        id_arr.append(mov_id)
        entry_data = (mov_id, 0, 0, "Movie not rejected", "Movie not yet reviewed", 0, 0, 0, 0, entry)
        data_cursor.execute("""INSERT OR REPLACE INTO movies VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?)""", entry_data)
        new_conn.commit()

    new_conn.close()


