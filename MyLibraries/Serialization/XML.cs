using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;

namespace Serialization
{
    public static class XML
    {
        public static bool IsPrimitiveType(Type obj)
        {
            if (Equals(obj, typeof(string))     || Equals(obj, typeof(char))
                || Equals(obj, typeof(byte))    || Equals(obj, typeof(sbyte))
                || Equals(obj, typeof(int))     || Equals(obj, typeof(uint))
                || Equals(obj, typeof(short))   || Equals(obj, typeof(ushort))
                || Equals(obj, typeof(long))    || Equals(obj, typeof(ulong))
                || Equals(obj, typeof(float))   || Equals(obj, typeof(double))
                || Equals(obj, typeof(decimal)) || Equals(obj, typeof(bool)))
                return true;
            return false;
        }

        public static string Serialize<T>(T value, string nameValue = "None", int shift = 0)
        {
            Type type = value.GetType();
            PropertyInfo[] properties = type.GetProperties();
            string result = new string('\t', shift) + "<" + type.Name + ">\n";
            result += new string('\t', shift + 1) + "<name>" + nameValue + "</name>\n";
            result += new string('\t', shift + 1) + "<value>\n";

            foreach (PropertyInfo property in properties)
            {
                // если примитивный тип, то сериализовать
                if (IsPrimitiveType(property.PropertyType))
                {
                    result += new string('\t', shift + 2) + "<" + property.PropertyType + ">\n";
                    result += new string('\t', shift + 3) + "<name>" + property.Name + "</name>\n";
                    result += new string('\t', shift + 3) + "<value>" + property.GetValue(value, null) + "</value>\n";
                    result += new string('\t', shift + 2) + "</" + property.PropertyType + ">\n";
                }
                // если составной тип, то рекурсия
                else
                {
                    object obj = Activator.CreateInstance(property.PropertyType);
                    obj = property.GetValue(value, null);
                    result += Serialize(obj, property.Name, shift + 2);
                }
            }

            result += new string('\t', shift + 1) + "</value>\n";
            return result + new string('\t', shift) + "</" + type.Name + ">\n";
        }

        public static T Deserialize<T>(string path)
        {
            StreamReader sr = new StreamReader(path);
            List<string> xml = new List<string>();
            while (!sr.EndOfStream)
            {
                xml.Add(sr.ReadLine());
            }
            sr.Close();
            int index = 0;
            return (T)_Deserialize(xml, null, ref index);
        }

        private static object _Deserialize(List<string> xml, object parent, ref int index)
        {
            for (int i = 0; i < xml.Count; ++i)
            {
                // проверка на корректность строки через Regex
                // либо тип, либо </value>

                if (xml[i].Contains("</value>")) // лучше убрать табуляцию и сравнить явно через Compare
                {
                    index = i;
                    break;
                }

                // выделяем тип
                var typeStr = xml[i].Replace("<", "").Replace(">", "").Replace("\t", "");
                var type = Type.GetType(typeStr);

                // если тип притивный, то инициализируем значение переменной
                if (IsPrimitiveType(type))
                {
                    // выделяем имя свойства + проверка
                    var propertyName = xml[i + 1].Replace("<name>", "").Replace("</name>", "").Replace("\t", "");
                    // выделяем значение свойства + проверка
                    var valueStr = xml[i + 2].Replace("<value>", "").Replace("</value>", "").Replace("\t", "");
                    foreach (var propertyInfo in parent.GetType().GetProperties())
                    {
                        if (propertyInfo.Name == propertyName)
                        {
                            // присваиваем выделенное значение данному свойству
                            var value = GetValueFromString(valueStr, type);
                            propertyInfo.SetValue(parent, value);
                        }
                    }
                    i += 3;
                    continue;
                }
                else
                {
                    object obj = Activator.CreateInstance(type);

                    // выделяем имя класса (если это имя главного родителя, то оно потеряется)
                    // + проверка
                    var className = xml[i + 1].Replace("<name>", "").Replace("</name>", "").Replace("\t", "");

                    var newXml = new List<string>(xml);

                    // если главный родитель
                    if (parent == null)
                    {
                        newXml.RemoveAt(0); // удаление <тип>
                        newXml.RemoveAt(0); // удаления <name> </name>
                        newXml.RemoveAt(0); // удаление <value>
                        return _Deserialize(newXml, obj, ref index); // parent = _Deserialize<T>(newXml, obj);
                    }
                    else
                    {
                        // обрезать newXml в рамках свойств этого сложного типа
                        for (int j = 0; j < i; j++)
                        {
                            newXml.RemoveAt(0);
                        }
                        newXml.RemoveAt(0); // удаление <тип>
                        newXml.RemoveAt(0); // удаления <name> </name>
                        newXml.RemoveAt(0); // удаление <value>

                        // инициализировать объект
                        _Deserialize(newXml, obj, ref index);

                        // найти нужное свойство в parent и записать туда значение из obj
                        foreach (var propertyInfo in parent.GetType().GetProperties())
                        {
                            if (propertyInfo.Name == className)
                            {
                                propertyInfo.SetValue(parent, obj);
                            }
                        }

                        // скипнуть нужное количество строк в xml
                        i = index + 2;
                    }
                }
            }

            return parent;
        }

        private static object GetValueFromString(string valueStr, Type type)
        {
            if (type == typeof(string))
                return valueStr;
            if (type == typeof(char))
                return valueStr[0];

            if (type == typeof(byte))
                return byte.Parse(valueStr);
            if (type == typeof(sbyte))
                return sbyte.Parse(valueStr);

            if (type == typeof(int))
                return int.Parse(valueStr);
            if (type == typeof(uint))
                return uint.Parse(valueStr);

            if (type == typeof(short))
                return short.Parse(valueStr);
            if (type == typeof(ushort))
                return ushort.Parse(valueStr);

            if (type == typeof(long))
                return long.Parse(valueStr);
            if (type == typeof(ulong))
                return ulong.Parse(valueStr);

            if (type == typeof(float))
                return float.Parse(valueStr);
            if (type == typeof(double))
                return double.Parse(valueStr);

            if (type == typeof(decimal))
                return decimal.Parse(valueStr);
            if (type == typeof(bool))
                return bool.Parse(valueStr);

            throw new ArgumentException();
        }
    }
}