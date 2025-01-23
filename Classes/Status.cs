using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Navigation;
using Newtonsoft.Json.Linq;

namespace Watcher.Classes
{
    public class Status
    {
        public static SolidColorBrush GREEN_COLOR = new BrushConverter().ConvertFromString("#FF82FFA7") as SolidColorBrush;
        public static SolidColorBrush RED_COLOR = new BrushConverter().ConvertFromString("#FF8282") as SolidColorBrush;
        public static SolidColorBrush YELLOW_COLOR = new BrushConverter().ConvertFromString("#FFFFBB82") as SolidColorBrush;
        public static SolidColorBrush INACTIVE = new BrushConverter().ConvertFromString("#FFE4E4E4") as SolidColorBrush;


        public static SolidColorBrush getStatus(int percent_of_workers, bool critical)
        {

            switch (DataManager.GetStatusSetting())
            {
                case "ONLY_CRITICAL":
                    if (critical)
                    {
                        return RED_COLOR;
                    }
                    else
                    {
                        return GREEN_COLOR;
                    }
                    break;
                case "STRICT":
                    if (percent_of_workers < 100)
                    {
                        return RED_COLOR;
                    }
                    else
                    {
                        return GREEN_COLOR;
                    }
                    break;
                case "CLUSTER_AND_CRITICAL":
                    if (critical) return RED_COLOR;

                    if (percent_of_workers < 30)
                    {
                        return RED_COLOR;
                    }
                    else if (percent_of_workers < 60)
                    {
                        return YELLOW_COLOR;
                    }
                    else
                    {
                        return GREEN_COLOR;
                    }

                    break;
                case "OFF":

                    return INACTIVE;

                    break;
            }
            return INACTIVE;
        }
    }
}
