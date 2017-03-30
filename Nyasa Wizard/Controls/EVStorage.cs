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

        static EVStorage()
        {
        }

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

        public static LazyControl<EVControlName> EvControlName = new LazyControl<EVControlName>();
        public static LazyControl<EVMemoryGame> EvMemoryGame = new LazyControl<EVMemoryGame>();
        public static LazyControl<EVControlScripts> EvControlScripts = new LazyControl<EVControlScripts>();
        public static LazyControl<EVReferencedImage> EvReferencedImage = new LazyControl<EVReferencedImage>();
        public static LazyControl<EVReferencedImage> EvReferencedImageB = new LazyControl<EVReferencedImage>();
        public static LazyControl<EVControlStyle> EvControlStyle = new LazyControl<EVControlStyle>();
        public static LazyControl<EVPageName> EvPageName = new LazyControl<EVPageName>();
        public static LazyControl<EVOrderedList> EvOrderedList = new LazyControl<EVOrderedList>();
    }

    public class LazyControl<T> where T : UserControl, new()
    {
        private T control = null;
        public T Instance
        {
            get
            {
                if (control == null)
                    control = new T();
                return control;
            }
        }
    }
}
