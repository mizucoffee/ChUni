﻿
using System.Collections.Generic;
using UnityEngine;

public class FileLoader{

    private string data;

    private string songname;
    private string artist;
    private int bpm;
    private int maxrhythm;

    private int start = 0;
    private int end = 0;
    private int notes = 0;

    private List<List<NotesModel>> list;

    public FileLoader(string text)
    {
        list = new List<List<NotesModel>>();
        data = text;
        string[] split = data.Split('\n');
        // この辺にちゃんとしたチェック挟む

        for (int i = 0; i < split.Length; ++i)
        {
            if (split[i] == ":START")
            {
                start = i + 1;
            }
            if (split[i] == ":END")
            {
                end = i;
            }
            if (split[i].StartsWith(":SONGNAME="))
            {
                songname = split[i].Substring(10);
            }
            if (split[i].StartsWith(":ARTIST="))
            {
                artist = split[i].Substring(8);
            }
            if (split[i].StartsWith(":BPM="))
            {
                // 数字チェック
                bpm = int.Parse(split[i].Substring(5));
            }
            if (split[i].StartsWith(":MAXRHYTHM="))
            {
                // 数字チェック
                maxrhythm = int.Parse(split[i].Substring(11));
            }
        }
        notes = end - start + 1;

        int space = 0;


        int now = start;
        int count = 0;
        while (now < end)
        {
            list.Add(new List<NotesModel>());
            string target = split[now];
            string tmp = "";
            int size = 0;
            int pos = 0;
            
            for (int i = 0; i < target.Length; ++i)
            {
                switch (tmp)
                {
                    case "":
                        switch (target.ToCharArray()[i])
                        {
                            case '-':
                                continue;
                            case 'R':
                                tmp = "R";
                                size = 1;
                                pos = i;
                                break;
                            case 'Y':
                                tmp = "Y";
                                size = 1;
                                pos = i;
                                break;
                            case 'L':
                                tmp = "L";
                                size = 1;
                                pos = i;
                                break;
                            case ':':
                                if (target.StartsWith(":RHYTHM="))
                                {
                                    space = maxrhythm / int.Parse(target.Substring(8)) - 1;
                                    i = target.Length;
                                    continue;
                                }
                                break;
                        }
                        break;
                    case "R":
                        switch (target.ToCharArray()[i])
                        {
                            case '2':
                            case 'R':
                                size++;
                                list[count].Add(new NotesModel(size, pos,0));
                                size = 0;
                                tmp = "";
                                break;
                            case '=':
                                size++;
                                break;
                            default:
                                list[count].Add(new NotesModel(size, pos, 0));
                                size = 0;
                                tmp = "";
                                break;
                        }
                        break;
                    case "Y":
                        switch (target.ToCharArray()[i])
                        {
                            case '2':
                            case 'Y':
                                size++;
                                list[count].Add(new NotesModel(size, pos, 1));
                                size = 0;
                                tmp = "";
                                break;
                            case '=':
                                size++;
                                break;
                            default:
                                list[count].Add(new NotesModel(size, pos, 1));
                                size = 0;
                                tmp = "";
                                break;
                        }
                        break;
                    case "L":
                        switch (target.ToCharArray()[i])
                        {
                            case '2':
                            case 'L':
                                size++;
                                int hold = 1;
                                for(int j = now + 1; j < end; j++)
                                {
                                    if (split[j].ToCharArray()[pos] == 'H' ||
                                        split[j].ToCharArray()[i] == 'H')
                                    {
                                        hold++;
                                        continue;
                                    }
                                    if (split[j].ToCharArray()[pos] == 'N' ||
                                       split[j].ToCharArray()[i] == 'N')
                                    {
                                        hold++;
                                        break;
                                    }
                                }

                                NotesModel nm = new NotesModel(size, pos, 2);
                                nm.setHold(hold);
                                list[count].Add(nm);
                                size = 0;
                                tmp = "";
                                break;
                            case '=':
                                size++;
                                break;
                            default:
                                list[count].Add(new NotesModel(size, pos, 0));
                                size = 0;
                                tmp = "";
                                break;
                        }
                        break;
                }
            }
            for (int i = 0; i < space; i++)
            {
                list.Add(new List<NotesModel>());
                count++;
            }
            now++;
            count++;
        }
    }
    public List<List<NotesModel>> getList()
    {
        return list;
    }
}
