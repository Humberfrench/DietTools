using System.Globalization;
using System.Reflection;
using System.Xml;

namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {

        public static string ToXml<T>(this IEnumerable<T> lista)
        {
            StringWriter sw = new StringWriter();
            XmlWriter xml = new XmlTextWriter(sw);

            xml.WriteStartElement("DocumentElement");
            foreach (T itemLista in lista.Where(itemLista => (object)itemLista != null))
            {
                xml.WriteStartElement("Tabela");
                Type tipoEntidade = typeof(T);
                List<FieldInfo> camposClasse = tipoEntidade.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                if (tipoEntidade.BaseType != null)
                    camposClasse.AddRange(tipoEntidade.BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));

                foreach (FieldInfo infoCampo in camposClasse.Where(infoCampo => infoCampo != null))
                {
                    object valor = infoCampo.GetValue(itemLista).TrataValores();
                    string nomeField = infoCampo.Name;

                    if (infoCampo.Name.Equals("_id"))
                        nomeField = "id";

                    if (valor != null)
                        xml.WriteElementString(nomeField, valor.ToString());

                }
                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            return sw.ToString();
        }

        /// <summary>
        /// Converte/Serializa um Generic type para XML.
        /// Diferente do ToXml, ele considera que existe itens filhos com mais ocorencias, e trata as mesmas gerando novo subnode do XML.
        /// Caso seja uma lista que tenha campos simples, considerar o ToXml() para uso, caso exista um node que tenha itens filhos, ou mais, usar este método.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lista"></param>
        /// <returns>String XML</returns>
        public static string ToXml2<T>(this IEnumerable<T> lista)
        {
            StringWriter sw = new StringWriter();
            XmlWriter xml = new XmlTextWriter(sw);

            xml.WriteStartElement("DocumentElement");
            foreach (T itemLista in lista.Where(itemLista => (object)itemLista != null))
            {
                xml.WriteStartElement("Tabela");
                Type tipoEntidade = typeof(T);
                List<FieldInfo> camposClasse = tipoEntidade.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                if (tipoEntidade.BaseType != null)
                    camposClasse.AddRange(tipoEntidade.BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));

                foreach (FieldInfo infoCampo in camposClasse.Where(infoCampo => infoCampo != null))
                {
                    if (infoCampo.FieldType != typeof(int) && infoCampo.FieldType != typeof(string) && infoCampo.FieldType != typeof(decimal) && infoCampo.FieldType != typeof(DateTime) && infoCampo.FieldType.IsGenericType) //CLASSE LISTA
                    {
                        object value = infoCampo.GetValue(itemLista);
                        List<FieldInfo> camposClasseFilho = value.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                        Array listaCamposFilhos = (Array)camposClasseFilho[0].GetValue(value);

                        foreach (object oItem in listaCamposFilhos)
                        {
                            if (oItem == null)
                                continue;

                            EscreverObjeto<T>(oItem, tipoEntidade, xml, infoCampo);
                        }
                    }
                    else
                    {
                        if (infoCampo.FieldType != typeof(int) && infoCampo.FieldType != typeof(string) && infoCampo.FieldType != typeof(decimal) && infoCampo.FieldType != typeof(DateTime)) //CLASSE NÂO LISTA
                        {
                            object value = infoCampo.GetValue(itemLista);
                            EscreverObjeto<T>(value, tipoEntidade, xml, infoCampo);
                        }
                        else //OBJETO SIMPLES
                        {
                            object valor = infoCampo.GetValue(itemLista).TrataValores();
                            string nomeField = infoCampo.Name;

                            if (infoCampo.Name.Equals("_id"))
                                nomeField = "id";

                            if (valor != null)
                                xml.WriteElementString(nomeField, valor.ToString());
                        }
                    }
                }

                xml.WriteEndElement();
            }

            xml.WriteEndElement();

            return sw.ToString();
        }

        private static void EscreverObjeto<T>(object oItem, Type tipoEntidade, XmlWriter xml, FieldInfo infoCampo)
        {
            List<FieldInfo> oItemFields = oItem.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

            if (oItem.GetType().BaseType != null)
                oItemFields.AddRange(tipoEntidade.BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));

            List<FieldInfo> oItemFieldsToAdd = new List<FieldInfo>();

            foreach (FieldInfo infoCampoFilhoList in oItemFields.Where(infoCampoFilhoList => infoCampoFilhoList != null))
            {
                object valorFilho = infoCampoFilhoList.GetValue(oItem).TrataValores();
                oItemFieldsToAdd.Add(infoCampoFilhoList);

            }

            xml.WriteStartElement(infoCampo.Name);

            foreach (FieldInfo infoCampoFilhoList in oItemFieldsToAdd)
            {
                object valorFilho = infoCampoFilhoList.GetValue(oItem).TrataValores();
                string nomeFieldFilho = infoCampoFilhoList.Name;

                if (infoCampoFilhoList.Name.Equals("_id"))
                    nomeFieldFilho = "id";

                if (valorFilho != null)
                    xml.WriteElementString(nomeFieldFilho, valorFilho.ToString());
            }

            xml.WriteEndElement();
        }

        private static object TrataValores(this object valor)
        {
            if (valor == null)
                valor = "";

            var tipo = valor.GetType();

            if (tipo.Equals(typeof(DateTime)))
            {
                var data = Convert.ToDateTime(valor);
                if (data.Hour == 0 && data.Minute == 0 && data.Second == 0 && data.Millisecond == 0)
                    valor = data.ToString("dd/MM/yyyy");
                else
                    valor = data.ToString("dd/MM/yyyy" + " " + CultureInfo.GetCultureInfo("pt-br").DateTimeFormat.LongTimePattern);
            }
            else if (tipo.Equals(typeof(decimal)))
                valor = Convert.ToDecimal(valor).ToString(CultureInfo.CurrentCulture);
            else if (tipo.Equals(typeof(bool)) || tipo.Equals(typeof(Boolean)))
                valor = Convert.ToInt32(valor);
            else if (tipo.IsEnum)
            {

            }
            else if (tipo.IsArray || tipo.IsGenericType) // listas
            {
                return null;
            }
            else if (!tipo.IsValueType && !tipo.Equals(typeof(String)))
            {
                if (tipo.BaseType.Name != "BaseNegocio`1")
                // se estiver usando o proxy para pegar o id da classe pai
                {
                    tipo = tipo.BaseType;

                    //valor = tipo.BaseType.InvokeMember("get_Id", BindingFlags.InvokeMethod, null, valor, null);
                }
                if (tipo.BaseType != null && tipo.BaseType.BaseType != null)
                {
                    var campo = (tipo.BaseType.Name == "BaseNegocio`1") ? tipo.BaseType.GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance) : tipo.BaseType.BaseType.GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance);

                    valor = campo.GetValue(valor);
                }
                else
                {
                    valor = "";
                }
            }
            return valor;
        }

    }
}
