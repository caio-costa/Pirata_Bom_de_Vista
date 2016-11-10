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

        TextAsset asset = Resources.Load("Legendas/legendas") as TextAsset;
        if (asset != null) {
            Stream s = new MemoryStream(asset.bytes);


            var serializer = new XmlSerializer(typeof(LegendaContainer));
            LegendaContainer temp = serializer.Deserialize(s) as LegendaContainer;
            s.Close();
            return temp;
        }

        return null;



        /*
        var serializer = new XmlSerializer(typeof(LegendaContainer));
        using (var stream = new FileStream("Assets/Resources/Legendas/legendas.xml", FileMode.Open))
        {
            //Debug.Log("Encontrou arquivo");
            LegendaContainer temp = serializer.Deserialize(stream) as LegendaContainer;            
            stream.Close();
            return temp;
        }
        */
    }

}
