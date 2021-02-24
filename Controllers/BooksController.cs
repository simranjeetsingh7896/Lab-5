using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Lab_5.Controllers
{
    public class BooksController : Controller
    {

        public IActionResult Index()
        {
            IList<Models.Book> bookList = new List<Models.Book>();

            //load books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();
            
            if (System.IO.File.Exists(path))
            {
                doc.Load(path);
                XmlNodeList books = doc.GetElementsByTagName("book");

                foreach (XmlElement b in books)
                {
                    Models.Book book = new Models.Book();
                    book.ID = Int32.Parse(b.GetElementsByTagName("id")[0].InnerText);
                    book.Title = b.GetElementsByTagName("title")[0].InnerText;
                    book.AuthorTitle = b.GetElementsByTagName("author")[0].Attributes["title"].Value;
                    book.FirstName = b.GetElementsByTagName("firstname")[0].InnerText;
                    if(b.GetElementsByTagName("middlename").Count > 0)
                    {
                        book.MiddleName = b.GetElementsByTagName("middlename")[0].InnerText;
                    } else
                    {
                        book.MiddleName = "";
                    }
                    book.LastName = b.GetElementsByTagName("lastname")[0].InnerText;

                    bookList.Add(book);
                }

            }
            return View(bookList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var book = new Models.Book();

            //load books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNode LastBook = doc.DocumentElement;

            //book's last ID
            XmlElement LastID = LastBook.LastChild["id"];
            book.ID = Int32.Parse(LastID.InnerText) + 1;

            return View(book);
        }

        [HttpPost]
        public IActionResult Create(Models.Book b)
        {
            //Load books.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();

            if (System.IO.File.Exists(path))
            {
                //If file exists, load it and create a new book
                doc.Load(path);

                //Create a new book
                XmlElement book = CreateBookElement(doc, b);

                //Get the root element and add a new book
                doc.DocumentElement.AppendChild(book);

                //bonus mark Truncate the number of books to 5 books, so if the number of books exceeds 5, remove the first book then save the file. If less than 5, just save the file.
                if (doc.DocumentElement.GetElementsByTagName("book").Count > 5)
                {
                    doc.DocumentElement.RemoveChild(doc.DocumentElement.FirstChild);
                }
                
            }
            else
            {
                //If file doesn't exist, create it and create a new book
                XmlNode dec = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.AppendChild(doc);
                XmlNode root = doc.CreateElement("books");

                //create a new boook
                XmlElement book = CreateBookElement(doc, b);
                root.AppendChild(book);
                doc.AppendChild(root);

                //bonus mark Truncate the number of books to 5 books, so if the number of books exceeds 5, remove the first book then save the file. If less than 5, just save the file.
                if (doc.DocumentElement.GetElementsByTagName("book").Count > 5)
                {
                    doc.DocumentElement.RemoveChild(doc.DocumentElement.FirstChild);
                }
            }

            doc.Save(path);
            return RedirectToAction("Index");
        }

        private XmlElement CreateBookElement(XmlDocument doc, Models.Book newBook)
        {
            XmlElement book = doc.CreateElement("book");
            XmlElement id = doc.CreateElement("id");
            id.InnerText = newBook.ID.ToString();
            XmlElement title = doc.CreateElement("title");
            title.InnerText = newBook.Title;

            XmlElement author = doc.CreateElement("author");
            XmlAttribute authortitle = doc.CreateAttribute("title");
            authortitle.Value = newBook.AuthorTitle;
            XmlElement firstname = doc.CreateElement("firstname");
            firstname.InnerText = newBook.FirstName;
            XmlElement middlename = doc.CreateElement("middlename");
            middlename.InnerText = newBook.MiddleName;
            XmlElement lastname = doc.CreateElement("lastname");
            lastname.InnerText = newBook.LastName;


            author.Attributes.Append(authortitle);
            author.AppendChild(firstname);
            author.AppendChild(middlename);
            author.AppendChild(lastname);


            book.AppendChild(id);
            book.AppendChild(title);
            book.AppendChild(author);

            return book;
        }
    }
}
