﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using YoutubeExtractor;

namespace YouTubeDownloaderMass
{
    class Program
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        static bool errorIndicator = false;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                int description;
                if (InternetGetConnectedState(out description, 0))
                {
                    if ((new Ping()).Send("www.youtube.com").Status == IPStatus.Success)
                    {
                        List<string> list = new List<string>();
                        foreach (string fileName in args)
                        {
                            if (File.Exists(fileName))
                            {
                                if (Path.GetExtension(fileName).Equals(".csv"))
                                {
                                    Message(string.Format("Чтение файла {0}.", fileName));
                                    try
                                    {
                                        using (StreamReader streamReader = new StreamReader(fileName))
                                        {
                                            list.Add(streamReader.ReadToEnd());
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        Message(string.Format("Файл {0}: {1}.", fileName, exception.Message), true);
                                    }
                                }
                                else
                                {
                                    Message(string.Format("Файл {0} должен иметь расширение '.csv'.", fileName), true);
                                }
                            }
                            else
                            {
                                Message(string.Format("Файл {0} не существует.", fileName), true);
                            }
                        }
                        if (list.Count != 0)
                        {
                            Console.WriteLine("Обработка содержимого файлов.");
                            string allText = string.Join("\n", list);
                            list = allText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            const string pattern = @"^(http(s)?\:\/\/)?(www\.)?youtube\.com\/watch\?v=([-_0-9a-zA-Z]){11}(&([-=_0-9a-zA-Z&])*)?;(144|360|480|720|1080);([a-zA-Z]:\\|[a-zA-Z]:(\\(\b[^ \\\/\*\:\?\<\>\|\""][^\\\/\*\:\?\<\>\|\""]*[^ \\\/\*\:\?\<\>\|\""]\b|[^ \\\/\*\:\?\<\>\|\""]))+|[a-zA-Z]:\\((\b[^ \\\/\*\:\?\<\>\|\""][^\\\/\*\:\?\<\>\|\""]*[^ \\\/\*\:\?\<\>\|\""]\b|[^ \\\/\*\:\?\<\>\|\""])\\)+)$";
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (!Regex.IsMatch(list[i], pattern))
                                {
                                    Message(string.Format("Строка {0} имеет недопустимый формат.", list[i]), true);
                                    list.RemoveAt(i);
                                    i--;
                                }
                            }
                            if (list.Count != 0)
                            {
                                List<Tuple<VideoInfo, string>> videoList = new List<Tuple<VideoInfo, string>>();
                                foreach (string item in list)
                                {
                                    string[] parts = item.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    parts[0] = parts[0].Substring(parts[0].IndexOf('=') + 1, 11);
                                    string url = string.Concat("https://www.youtube.com/watch?v=", parts[0]);
                                    Message(string.Format("Поиск видео по URL {0}.", url));
                                    int resolution = Convert.ToInt32(parts[1]);
                                    for (int i = 0; i < 5; i++)
                                    {
                                        try
                                        {
                                            VideoInfo video = DownloadUrlResolver.GetDownloadUrls(url)
                                                                                      .OrderByDescending(p => p.Resolution)
                                                                                      .FirstOrDefault(p => p.VideoType == VideoType.Mp4 &&
                                                                                                        p.Resolution <= resolution &&
                                                                                                        p.AudioType != AudioType.Unknown);
                                            if (video != null)
                                            {
                                                videoList.Add(new Tuple<VideoInfo, string>(video, parts[2]));
                                            }
                                            else
                                            {
                                                Message(string.Format("Видео по URL {0} не найдено.", url), true);
                                            }
                                            break;
                                        }
                                        catch (YoutubeParseException)
                                        {
                                            if (i == 4)
                                            {
                                                Message(string.Format("Видео по URL {0} не найдено.", url), true);
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            Message(string.Format("URL: {0}: {1}", url, exception.Message), true);
                                            break;
                                        }
                                    }
                                }
                                list.Clear();
                                if (videoList.Count != 0)
                                {
                                    char[] invalidChars = Path.GetInvalidFileNameChars();
                                    for (int i = 0; i < videoList.Count; i++)
                                    {
                                        try
                                        {
                                            string title = videoList[i].Item1.Title;
                                            foreach (char item in invalidChars)
                                            {
                                                if (title.IndexOf(item) != -1)
                                                {
                                                    title = title.Replace(item, '_');
                                                }
                                            }
                                            VideoDownloader downloader = new VideoDownloader(videoList[i].Item1, Path.Combine(videoList[i].Item2, title + videoList[i].Item1.VideoExtension));
                                            downloader.DownloadStarted += downloader_DownloadStarted;
                                            downloader.DownloadFinished += downloader_DownloadFinished;
                                            if (!Directory.Exists(videoList[i].Item2))
                                            {
                                                Directory.CreateDirectory(videoList[i].Item2);
                                            }
                                            downloader.Execute();
                                        }
                                        catch (WebException exception)
                                        {
                                            if (exception.Status == WebExceptionStatus.ProtocolError)
                                            {
                                                HttpWebResponse response = exception.Response as HttpWebResponse;
                                                switch (response.StatusCode)
                                                {
                                                    case HttpStatusCode.Forbidden:
                                                        Message(string.Format("Видео {0}: Доступ запрещен.", videoList[i].Item1.Title), true);
                                                        break;
                                                    default:
                                                        Message(string.Format("Видео {0}: Неизвестная ошибка.", videoList[i].Item1.Title), true);
                                                        break;
                                                }
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            Message(string.Format("Видео {0}: {1}", videoList[i].Item1.Title, exception.Message), true);
                                        }
                                    }
                                    videoList.Clear();
                                }
                                else
                                {
                                    Message("Нет данных для загрузки видео.", true);
                                }
                            }
                            else
                            {
                                Message("Нет данных для поиска видео.", true);
                            }
                        }
                        else
                        {
                            Message("Нет данных для обработки.", true);
                        }
                    }
                    else
                    {
                        Message(@"URL https://www.youtube.com недоступен.", true);
                    }
                }
                else
                {
                    Message("Отсутствует соединение с сетью Интернет.", true);
                }
            }
            else
            {
                Message("Отсутствует параметр 'Путь к файлу'.", true);
            }
            if (errorIndicator)
            {
                Message("Операция завершена с ошибками.");
            }
            else
            {
                Message("Операция успешно завершена.");
            }
            Console.ReadKey();
        }

        private static void downloader_DownloadStarted(object sender, EventArgs e)
        {
            VideoDownloader downloader = sender as VideoDownloader;
            Message(string.Format("Загрузка видео {0}.", downloader.Video.Title));
        }

        static void downloader_DownloadFinished(object sender, EventArgs e)
        {
            VideoDownloader downloader = sender as VideoDownloader;
            Message(string.Format("Видео загружено в {0}.", downloader.SavePath));
        }

        private static void Message(string message, bool error = false)
        {
            Console.WriteLine(message);
            if (!errorIndicator && error)
            {
                errorIndicator = error;
            }
        }
    }
}
