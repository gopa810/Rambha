using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlideMaker
{
    public class EVStorage
    {
        public class UserControlEntry
        {
            public Type ControlType = null;
            public UserControl Control = null;
            public bool locked = false;
        }

        private static List<UserControlEntry> Controls = new List<UserControlEntry>();

        public static UserControl GetUserControl(Type t)
        {
            if (t.IsSubclassOf(typeof(UserControl)))
            {
                foreach (UserControlEntry uce in Controls)
                {
                    if (uce.ControlType == t && uce.locked == false && uce.Control != null)
                    {
                        uce.locked = true;
                        return uce.Control;
                    }
                }
            }

            return null;
        }

        public static UserControl GetSafeControl(Type t)
        {
            UserControl emg = EVStorage.GetUserControl(t);
            if (emg == null)
            {
                emg = (UserControl)Activator.CreateInstance(t);
                EVStorage.AddUserControl(emg);
            }
            return emg;
        }


        public static void AddUserControl(UserControl uc)
        {
            UserControlEntry uce = new UserControlEntry();
            uce.Control = uc;
            uce.ControlType = uc.GetType();
            uce.locked = true;
        }

        public static void ReleaseUserControl(UserControl uc)
        {
            foreach (UserControlEntry uce in Controls)
            {
                if (uce.Control == uc)
                {
                    uce.locked = false;
                    return;
                }
            }
        }
    }
}
