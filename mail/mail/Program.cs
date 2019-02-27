using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Net.Sockets;
using OpenPop.Mime;
using OpenPop.Mime.Header;
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using OpenPop.Common.Logging;
using MailKit;
using MailKit.Net.Imap;

namespace mail
{
    class Mail
    {
        private Pop3Client Client_pop3 = new OpenPop.Pop3.Pop3Client();
        ImapClient Client_imap = new ImapClient();
        private Dictionary<int, Message> Messages = new Dictionary<int, Message>();
        private string Add, Password, Pop ,Smtp, Imap;
        public Mail(){Add="пусто";}

        void Set_add()
        {
            Console.WriteLine("Введите imap");
            string add = Console.ReadLine();
            Add= add;
        }
        string Get_add() { return Add; }

        void Set_imap()
        {
            Console.WriteLine("введите pop");
            var imap = Console.ReadLine();
            Imap = imap;
        }

        void Set_pop()
        {
            Console.WriteLine("введите pop");
            var pop = Console.ReadLine();
            Pop = pop;
        }

        void Set_smtp()
        {
            Console.WriteLine("введите smpt");
            var smtp = Console.ReadLine();
            Smtp = smtp;
        }

        void Set_pass()
        {
            Console.WriteLine("Введите пароль электроной почты");
            string inpt;
            inpt = string.Empty;
            while (true)
            {
                var key = Console.ReadKey(true);//не отображаем клавишу - true

                if (key.Key == ConsoleKey.Enter) break; //enter - выходим из цикла

                Console.Write("*");//рисуем звезду вместо нее
                inpt += key.KeyChar; //копим в пароль символы
            }
            Console.WriteLine();
            Password = inpt;
        }

        void Conection_pop3()
        {
            try {
                Client_pop3.Connect(Pop, 995, true);
                Client_pop3.Authenticate(Add, Password, AuthenticationMethod.UsernameAndPassword);
                if (Client_pop3.Connected)
                {
                    Console.WriteLine("вы подключены");
                }
            }
            catch
            {
                Console.WriteLine("неправельные данные");
            }
        }

        void Send_mes_smtp()
        {
            try
            {
                MailMessage Message = new MailMessage();
                Message.From = new System.Net.Mail.MailAddress(Add);
                Console.WriteLine("куда хотите отправить сообщение");
                var add2 = Console.ReadLine();
                Message.To.Add(new System.Net.Mail.MailAddress(add2));
                Console.WriteLine("введите тему сообщения");
                var teme = Console.ReadLine();
                Console.WriteLine("введите тему сообщения");
                var masseg = Console.ReadLine();
                Message.Subject = teme;
                Message.Body = "сообщение";
                SmtpClient client = new SmtpClient(Smtp, 25);
                client.Port = 587;
                client.Credentials = new NetworkCredential(Add, Password);
                client.EnableSsl = true;
                try
                {
                    client.Send(Message);
                }
                catch (SmtpException)
                {
                    Console.WriteLine("Ошибка!");
                }
            }
            catch
            {
                Console.WriteLine("необходимые данные не введены");
            }
        }

        void Take_mes_pop3()
        {
            try
            {
                Conection_pop3();
                int count = Client_pop3.GetMessageCount();
                for (int i = count; i >= 1; i--)
                {
                    string body;
                    Message message = Client_pop3.GetMessage(i);
                    MessagePart plainTextPart = message.FindFirstPlainTextVersion();
                    if (plainTextPart != null)
                    {
                        // The message had a text/plain version - show that one
                        body = plainTextPart.GetBodyAsText();
                    }
                    else
                    {
                        // Try to find a body to show in some of the other text versions
                        List<MessagePart> textVersions = message.FindAllTextVersions();
                        if (textVersions.Count >= 1)
                            body = textVersions[0].GetBodyAsText();
                        else
                            body = "<<OpenPop>> Cannot find a text version body in this message to show <<OpenPop>>";
                    }
                    // Build up the attachment list
                    List<MessagePart> attachments = message.FindAllAttachments();
                    foreach (MessagePart attachment in attachments)
                    { }

                    // Add the message to the dictionary from the messageNumber to the Message
                    Messages.Add(i, message);
                }
                Console.WriteLine("подключение осуществлено. на почте " + count + " писем");
            }
            catch
            {
                Console.WriteLine("необходимые данные не введены");
            }
        }

        void Take_mes_imap()
        {
            Client_imap.Connect(Imap,993,true);
            Client_imap.Authenticate(Add,Password);
            IMailFolder inbox = Client_imap.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            Console.WriteLine("Входящие {0}", inbox.Count);
            Console.WriteLine("введите номер сообщения которое вы хотите получить");
            try
            {
                var i = Convert.ToInt32(Console.ReadLine());
                var massage = inbox.GetMessage(i);
                Console.WriteLine("Subject: {0}", massage.TextBody);
            }
            catch
            {
                Console.WriteLine("неправельно введены данные");
            }
        }

        public void Init()
        {
            string i;
            while(true)
            {
                Console.WriteLine("Элементы управления"+ "\r\n"+ "1)ввести логин" + "\r\n" + " 2)ввести пароль" + "\r\n" + " 3)ввести pop" + "\r\n" + " 4)ввести smtp" + "\r\n"  +
                    " 5)ввести imap" + "\r\n" + " 6)получить сообщения pop3" + "\r\n" + " 7)получить сообщения imap" + "\r\n"+ " 8)отправить сообщение" + "\r\n" + " 9)выход");
                i = Console.ReadLine();
                if (i=="1")
                {
                    Set_add();
                }
                if (i == "2")
                {
                    Set_pass();
                }
                if (i == "3")
                {
                    Set_pop();
                }
                if (i == "4")
                {
                    Set_smtp();
                }
                if (i == "5")
                {
                    Set_imap();
                }
                if (i == "6")
                {
                    Take_mes_pop3();
                }
                if (i == "7")
                {
                    Take_mes_imap();
                }
                if (i == "8")
                {
                    Send_mes_smtp();
                }
                if (i == "9")
                {
                    break;
                }
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var a = new Mail();
            a.Init();
        }
    }
}

