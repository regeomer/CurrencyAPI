using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyAPI.Infrastracture
{
    public abstract class SingletonBase<T> where T : new()
    {
        private static T m_Instance;

        static SingletonBase()
        {
            m_Instance = new T();
        }

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new T();
                }

                return m_Instance;
            }
        }
    }
}
