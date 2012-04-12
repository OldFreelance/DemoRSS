using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace DemoRSS
{
    class MyRss
    {
        /// <summary>
        /// Получить список записей RSS по заданному адресу
        /// </summary>
        /// <param name="url">Адрес рассылки</param>
        /// <returns>Список RSS записей</returns>
        public static List<RssEntry> LoadRss(string url)
        {
            List<RssEntry> entries = new List<RssEntry>();

            using (WebClient webClient = new WebClient())
            {
                MemoryStream stream = new MemoryStream(webClient.DownloadData(url));
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);

                foreach (XmlNode channel in doc.DocumentElement.ChildNodes)
                {
                    switch (channel.Name)
                    {
                        //Если рассылка содержит каналы
                        case "channel":
                            //Разбиваем на канал на записи
                            foreach (XmlNode item in channel.ChildNodes)
                            {
                                if (item.Name == "item")
                                {
                                    entries.Add(GetRssEntry(item));
                                }
                            }
                            break;
                        //Если рассылка не содержит каналы то просто парсим записи
                        case "entry":
                            entries.Add(GetRssEntry(channel));
                            break;
                    }
                }
            }

            return entries;
        }

        /// <summary>
        /// Получение RSS записи из дочерних записей узла xml
        /// </summary>
        /// <param name="item">Узел xml</param>
        /// <returns>RSS запись</returns>
        private static RssEntry GetRssEntry(XmlNode item)
        {
            RssEntry rssEntry = new RssEntry();

            foreach (XmlNode field in item.ChildNodes)
            {
                switch (field.Name.ToLower())
                {
                    case "id":
                        rssEntry.Id = field.InnerText;
                        break;
                    case "guid":
                        rssEntry.Guid = field.InnerText;
                        break;
                    case "title":
                        rssEntry.Title = field.InnerText;
                        break;
                    case "link":
                        rssEntry.Link = field.InnerText;
                        break;
                    case "dc:creator":
                    case "author":
                        rssEntry.Author = field.InnerText;
                        break;
                    case "content:encoded":
                    case "content":
                        rssEntry.Content = field.InnerText;
                        break;
                    case "summary":
                        rssEntry.Summary = field.InnerText;
                        break;
                    case "description":
                        rssEntry.Description = field.InnerText;
                        break;
                    case "published":
                    case "pubdate":
                        rssEntry.PubDate = field.InnerText;
                        break;
                    case "updated":
                        rssEntry.Updated = field.InnerText;
                        break;
                    case "wfw:commentrss":
                    case "slash:comments":
                    case "comments":
                        rssEntry.Comments = field.InnerText;
                        break;
                    case "category":
                        rssEntry.Category = field.InnerText;
                        break;
                    case "mash:thumbnail":
                    case "feedburner:origlink":
                        break;
                    //default:
                        //throw new Exception("Необрабатываемое поле рассылки - "+field.Name +" ("+field.InnerText+")");
                }
            }

            return rssEntry;
        }
    }
}
