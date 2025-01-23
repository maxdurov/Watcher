using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watcher.Classes
{
    public class EventsHandler
    {
        List<string> events;

        public delegate void NewEventHandler(string data);
        static public event NewEventHandler NewEvent;

        public EventsHandler()
        {
            events = new List<string>();
            AddEvent("START");
        }

        public void AddEvent(string data)
        {
            if (events.Count >= 50000) events.RemoveAt(0);
            string prepare_text = "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] \t" + data + "\n";
            events.Add(prepare_text);
            NewEvent?.Invoke(prepare_text);
        }

        public string GetEvents()
        {
            string a = "";

            foreach (var text in events)
            {
                a += text;
            }
            return a;
        }
    }
}
