using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class ObjetoLegenda {

    [XmlAttribute("indice")]
    public int indice;
    public string texto;
    public float duracao;
}
