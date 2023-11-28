﻿using TaskMessage.Enum;

namespace TaskMessage.Model
{
    public class Template
    {
        public int Id { get; set; }
        public Channel Channel { get; set; }
        public string Sender { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public string Subject { get; set; }
        public string Attachments { get; set; }
    }
}
