﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ITMO.JSON.MyParser
{
    public static class MyJsonParser
    {
        private static string fulltext;
        private static List<string> elementGlobal;


        public static List<dynamic> JsonParser(string namefile)
        {
            fulltext = ReadFile(namefile);
            DeleteSpace();
            elementGlobal = Parse();

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            List<dynamic> listDynamic = new List<dynamic>();

            while (elementGlobal.Count > 0)
            {
                KeyValuePair<string, object> element = ElementPara();
                dictionary.Add(element.Key, element.Value);
                if ("}]" == elementGlobal[0])
                {
                    dynamic dynamicObj = new Expando();
                    foreach (var valueDictionary in dictionary)
                    {
                        dynamicObj.Add(valueDictionary.Key, valueDictionary.Value);
                    }
                    listDynamic.Add(dynamicObj);
                    dictionary = new Dictionary<string, object>();
                }
                elementGlobal.RemoveAt(0);
            }
            return listDynamic;
        }

        private static KeyValuePair<string, object> ElementPara()
        {
            string temp = "";
            Object val;
            KeyValuePair<string, object> myPair;

            temp = elementGlobal[0].Remove(0, elementGlobal[0].IndexOf("\"") + 1);
            string key = temp.Substring(0, temp.IndexOf("\""));
            temp = temp.Remove(0, temp.IndexOf(":") + 1);

            if (temp.Contains('['))
            {                
                val = ToListType(temp);
                myPair = new KeyValuePair<string, object>(key, val);
                return myPair;
            }
            else if (temp.Contains("{"))
            {
                dynamic dynamicObj =  ToPersonalType(temp); 
                myPair = new KeyValuePair<string, object>(key, dynamicObj);
                return myPair;
            }
            else if (temp.Contains("}]") || temp.Contains("} ]"))
            {
                temp = temp.Substring(0, temp.IndexOf("}"));
                elementGlobal[0] = "}]";
            }         
            else if (temp.Contains('}'))
            {
                temp = temp.Substring(0, temp.IndexOf("}"));
                elementGlobal[0] = "}";
            }

            val = ToObjectSTR(temp);
            myPair = new KeyValuePair<string, object>(key, val);
            return myPair;
        }

        private static Object ToListType(string str)
        {
            elementGlobal[0] = str;
            List<dynamic> listDynamics = new List<dynamic>();
            while ("}]" != elementGlobal[0])
            {
                dynamic dynamicObj = ToPersonalType(str);                             
                listDynamics.Add(dynamicObj);
                if ("}]" == elementGlobal[0])
                {
                    break;
                }
                elementGlobal.RemoveAt(0);
            }                    
            return listDynamics;
        }

        private static Object ToPersonalType(string str)
        {           
            elementGlobal[0] = str;
            Dictionary<string, object> dictionaryInner = new Dictionary<string, object>();
            while ("}" != elementGlobal[0])
            {
                KeyValuePair<string, object> element = ElementPara();
                dictionaryInner.Add(element.Key, element.Value);
                if ("}" == elementGlobal[0] || "}]" == elementGlobal[0])
                {
                    break;
                }
                elementGlobal.RemoveAt(0);
            }
            dynamic dynamicObj = new Expando();
            foreach (var valueDictionary in dictionaryInner)
            {
                dynamicObj.Add(valueDictionary.Key, valueDictionary.Value);
            } 
            return dynamicObj;
        }

        private static Object ToObjectSTR(string str)
        {
            Object val;          
            int resInt;
            double resDouble;  
            if(str[0] == ' ')
            {
                str = str.Substring(1); 
            }

            if (str.Contains('\"'))
            {
                val = str.Substring(str.IndexOf("\""), str.LastIndexOf("\"") - 1); ;
            }
            else if (str == "true") val = true;
            else if (str == "false") val = false;
            else if (Int32.TryParse(str, out resInt)) val = resInt;
            else if (Double.TryParse(str, out resDouble)) val = resDouble;
            else
            {
                val = (object)str;
            }
            return val;
        }   


        private static string ReadFile(string namefile)
        {
            string fulltext;

            using (var file = new StreamReader(namefile))
            {
                fulltext = file.ReadToEnd();
            }
            return fulltext;
        }

        private static List<string> Parse()
        {            
            List<string> list = new List<string>();

            string tempFullText = fulltext;
            string tempfragment;
            while (true)
            {
                int splitFragment1 = tempFullText.IndexOf(",\"");
                int splitFragment2 = tempFullText.IndexOf("},{");
                if ((splitFragment1 == -1) && (splitFragment2 == -1))
                {
                    list.Add(tempFullText);
                    break;
                }
                else if ((splitFragment1 < splitFragment2) || (splitFragment1 != -1) && (splitFragment2 == -1))
                {
                    tempfragment = tempFullText.Substring(0, tempFullText.IndexOf(",\""));
                    tempFullText = tempFullText.Remove(0, splitFragment1 + 1);
                }
                else 
                {
                    tempfragment = tempFullText.Substring(0, tempFullText.IndexOf("},{")+1);
                    tempFullText = tempFullText.Remove(0, splitFragment2+2);
                }         
                list.Add(tempfragment);            
            }  
            return list;
        }

        private static string DeleteSpace()
        {
            fulltext = fulltext.Replace("  ", "");
            fulltext = fulltext.Replace("\n", "");
            fulltext = fulltext.Replace("\r", "");          
            return fulltext;
        }
    }
}
