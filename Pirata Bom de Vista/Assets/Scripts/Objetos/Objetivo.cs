using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TipoObjetivo {
    CUTSCENE,
    LUGAR,
    COMBATE,
    NAVEGACAO
}

[Serializable]
public class SaveTransform {
    public Vector3 position = new Vector3();
    public Quaternion rotation = new Quaternion();
    public Vector3 localScale = new Vector3();
}

[Serializable]
public class Objetivo {
    public string Titulo = "";
    public string Descricao = "";
    public TipoObjetivo Tipo = TipoObjetivo.CUTSCENE;
    public bool podeSerCarregado = true;
    public Transform posCarregamento;
    [HideInInspector]
    public SaveTransform posCarregamentoSave = new SaveTransform();
    [HideInInspector]
    public bool transformSalvo = false;
}

[Serializable]
public class ListaObjetivos{
    public List<Objetivo> TodosObjetivos;

    public Objetivo GetObjetivo(int indice) {
        return TodosObjetivos[indice];
    }
}

[Serializable]
public class FaseAtual {
    public int fase = 0;
    public int indice = 0;
}
