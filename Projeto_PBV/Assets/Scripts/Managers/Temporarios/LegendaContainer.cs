using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("LegendaCollections")]
public class LegendaContainer {

    [XmlArray("Textos"), XmlArrayItem("ObjetoTexto")]
    public List<ObjetoLegenda> Legendas = new List<ObjetoLegenda>();
    //public string path = "Resources/Legendas/ObjetoTexto.xml";


    public static LegendaContainer Load()
    {
        var serializer = new XmlSerializer(typeof(LegendaContainer));
        using (var stream = new FileStream("Assets/Resources/Legendas/legendas.xml", FileMode.Open))
        {
            LegendaContainer temp = serializer.Deserialize(stream) as LegendaContainer;
            stream.Close();
            return temp;
        }

    }

}
