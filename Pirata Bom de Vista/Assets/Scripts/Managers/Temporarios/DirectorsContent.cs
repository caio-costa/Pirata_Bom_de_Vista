using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

#region Classes

[Serializable]
public class Conteudo {
    public List<FaseCutscene> todasFases = new List<FaseCutscene>();


    [Serializable]
    public class FaseCutscene {
        [SerializeField]
        private string nomeFase = "";
        public List<Cutscene> cutscene = new List<Cutscene>();
    }


    [Serializable]
    public class Cutscene {
        public string tituloCutscene = "";
        public List<CorteSave> cortes = new List<CorteSave>();
        //private int numeroCortes = 0;
    }
}

[Serializable]
public class ConteudoAnim
{
    public List<FaseCutscene> todasFases = new List<FaseCutscene>();


    [Serializable]
    public class FaseCutscene
    {
        [SerializeField]
        public List<Cutscene> cutscene = new List<Cutscene>();
    }


    [Serializable]
    public class Cutscene
    {
        public List<CorteAnimacao> cortes = new List<CorteAnimacao>();
        //private int numeroCortes = 0;
    }
}


[Serializable]
public class ConteudoAtual {
    public int fase = 0;
    public int cutscene = 0;
    public int corte = 0;
}

[Serializable]
public class CorteSave
{
    public ConteudoAtual posicaoEmLista = new ConteudoAtual();
    [HideInInspector]
    public Vector3 posicao;
    [HideInInspector]
    public Quaternion rotacao;
    public bool delay = false, fixa = true, velocidadeGradativa = false;
    public float tempoDelay = 0, tempoViagem = 0, delayTermino = 0;
    public Transform lookAt = null, posFinal = null;
}

[Serializable]
public class AdicaoCorte
{
    public int novoIndice = 0;
    public bool addCorte = false, zerarObjeto = false;
    public CorteSave corteSelecionado = new CorteSave();
}

[Serializable]
public class SerializableCorteSave
{

    public SerializableVector3 posicao;
    public SerializableQuaternion rotacao;
    public bool delay = false, fixa = true;
    public float tempoDelay = 0, tempoViagem = 0, delayTermino = 0;
    public int lookAt_ID;
    public int posFinal_ID;
}

[Serializable]
public class SerializableVector3
{
    public float x = 0, y = 0, z = 0;
}

[Serializable]
public class SerializableQuaternion
{
    public float x = 0, y = 0, z = 0, w = 0;
}
#endregion

[ExecuteInEditMode]
public class DirectorsContent : MonoBehaviour {

    public static DirectorsContent instance;
    [Header("Conteúdo das cutscenes")]
    public Conteudo conteudo = new Conteudo();

    #region Variaveis

    [Header("Configurações de camera atual")]
    [Tooltip("Salva a posição inicial da camera")]
    public bool setPosInicial = false;
    [Tooltip("Define a posição da camera inicial salva")]
    public bool voltaCameraInicial = false;

    [Header("Configurações do corte atual")]
    public bool addCorte = false;
    public bool deletarTudo = false;
    public bool zerarObjeto = false;


    public CorteSave corteAtual = new CorteSave();


    [SerializeField]
    [HideInInspector]
    private CorteSave posInicialCamera = new CorteSave();


    private string diretorioListagem = "C:/Leonardo/ListaCenas.txt";
    private string diretorioSave = "C:/Leonardo/Trabalho/Unity/Projetos_Unity/Pirata Bom de Vista/Backups/Cortes/";

    #endregion

    void Start() {
        instance = this;
    }

    void Update () {
#if UNITY_EDITOR
        #region InputOriginal
        if (addCorte)
        {
            addCorte = false;
            AddNovaPos();
        }else if (deletarTudo)
        {
            deletarTudo = false;
            if (EditorUtility.DisplayDialog("Deletar Lista de cortes", "Deseja deletar a lista de cortes?", "Deletar", "Não deletar"))
            {
                DeletarListaCompleta();
            }

        }
        else if (zerarObjeto)
        {
            zerarObjeto = false;
            ZerarObjetoAtual();
        }else if (setPosInicial)
        {
            setPosInicial = false;
            posInicialCamera.rotacao = Camera.main.transform.rotation;
            posInicialCamera.posicao = Camera.main.transform.position;
        }
        else if (voltaCameraInicial)
        {
            voltaCameraInicial = false;
            Camera.main.transform.position = posInicialCamera.posicao;
            Camera.main.transform.rotation = posInicialCamera.rotacao;
        }

        #endregion

        #region EditorHandler
        if (!corteAtual.delay) {
            if (corteAtual.tempoDelay != 0) {
                if (EditorUtility.DisplayDialog("Ativar Delay", "Para alterar o tempo de delay, ative-o na variável.", "Ativar", "Não Ativar"))
                {
                    corteAtual.delay = true;
                }
                else {
                    corteAtual.delay = false;
                }
            }

            corteAtual.tempoDelay = 0;

        }


        if (corteAtual.fixa == true) {
            if (corteAtual.lookAt != null || corteAtual.posFinal != null) {
                if (EditorUtility.DisplayDialog("Desativar camera fixa", "Para configurar um target ou um movimento, desativar a câmera fixa.", "Desativar", "Deixar ativa"))
                {
                    corteAtual.fixa = false;
                }
            }

            if (corteAtual.tempoViagem != 0)
            {
                if (corteAtual.fixa == true) {
                    if (EditorUtility.DisplayDialog("Ativar Viagem", "Para alterar o tempo de viagem, desative a câmera fixa.", "Desativar", "Deixar ativa"))
                    {
                        corteAtual.fixa = false;
                    }
                    else
                    {
                        corteAtual.fixa = false;
                    }
                }
                
            }

            corteAtual.tempoViagem = 0;
            corteAtual.lookAt = null;
            corteAtual.posFinal = null;
        }

        if (corteAtual.tempoViagem != 0)
        {
            if (corteAtual.posFinal == null)
            {
                if (EditorUtility.DisplayDialog("Adicionar Alvo", "Para alterar o tempo de viagem, adicione uma posição final.", "Ok"))
                {
                }

                corteAtual.tempoViagem = 0;
            }
        }

        #endregion
#endif
    }

    #region f_Original

    private void DeletarListaCompleta()
    {
        conteudo.todasFases.Clear();
        Debug.Log("Lista deletada");
        ZerarObjetoAtual();
    }

    private void AddNovaPos()
    {
#if UNITY_EDITOR
        if (Camera.main != null)
        {
            CorteSave objetoTemp = CopiaCorte(corteAtual);
            objetoTemp.posicao = Camera.main.transform.position;
            objetoTemp.rotacao = Camera.main.transform.rotation;

            conteudo.todasFases[objetoTemp.posicaoEmLista.fase].cutscene[objetoTemp.posicaoEmLista.cutscene].cortes.Insert(objetoTemp.posicaoEmLista.corte, objetoTemp);

            //posicoesCorte.Add(objetoTemp);
            //Debug.Log("Posicao Adicionada com sucesso." + "\nIndice: " + (numeroPos - 1).ToString() + "\nPos: " + objetoTemp.posicao + "\nRot: " + objetoTemp.rotacao);
            ZerarObjetoAtual();
        }
        else
        {
            Debug.LogError("Camera não encontrada");
        }
#endif
    }

    private void ZerarObjetoAtual()
    {
#if UNITY_EDITOR
        corteAtual = new CorteSave();
#endif
    }

    #endregion


    #region GettersSetters
    public CorteSave GetCorte(int fase, int cutscene, int corte) {
        if (conteudo.todasFases.Count > fase) {
            if (conteudo.todasFases[fase].cutscene.Count > cutscene) {
                if (conteudo.todasFases[fase].cutscene[cutscene].cortes.Count > corte)
                {
                    CorteSave corteTemp = CopiaCorte(conteudo.todasFases[fase].cutscene[cutscene].cortes[corte]);

                    return corteTemp;
                }
            }
        }

        Debug.Log("Indice Invalido!");
        return null;
    }


    public CorteSave GetCorte(ConteudoAtual conteudoTemp)
    {
        if (conteudo.todasFases.Count > conteudoTemp.fase)
        {
            if (conteudo.todasFases[conteudoTemp.fase].cutscene.Count > conteudoTemp.cutscene)
            {
                if (conteudo.todasFases[conteudoTemp.fase].cutscene[conteudoTemp.cutscene].cortes.Count > conteudoTemp.corte)
                {
                    CorteSave corteTemp = CopiaCorte(conteudo.todasFases[conteudoTemp.fase].cutscene[conteudoTemp.cutscene].cortes[conteudoTemp.corte]);

                    return corteTemp;
                }
            }
        }

        //Debug.Log("Indice Invalido!");
        return null;
    }


    public int GetNumeroFases() {
        return conteudo.todasFases.Count;
    }

    public int GetNumeroCutscenes(ConteudoAtual conteudoTemp) {
        return conteudo.todasFases[conteudoTemp.fase].cutscene.Count;
    }

    public int GetNumeroCortes(ConteudoAtual conteudoTemp) {
        return conteudo.todasFases[conteudoTemp.fase].cutscene[conteudoTemp.cutscene].cortes.Count;
    }

    public int GetCorteAtual() {
        return corteAtual.posicaoEmLista.corte;

    }
    #endregion

    #region Converters

    public SerializableVector3 ConverterVector3(Vector3 objeto) {
        SerializableVector3 data = new SerializableVector3();

        data.x = objeto.x;
        data.y = objeto.y;
        data.z = objeto.z;

        return data;
    }

    public Vector3 ConverterVector3(SerializableVector3 objeto) {
        Vector3 data = new Vector3();

        data.x = objeto.x;
        data.y = objeto.y;
        data.z = objeto.z;

        return data;
    }

    public SerializableQuaternion ConverterQuaternion(Quaternion objeto) {
        SerializableQuaternion data = new SerializableQuaternion();

        data.x = objeto.x;
        data.y = objeto.y;
        data.z = objeto.z;
        data.w = objeto.w;

        return data;
    }

    public Quaternion ConverterQuaternion(SerializableQuaternion objeto)
    {
        Quaternion data = new Quaternion();

        data.x = objeto.x;
        data.y = objeto.y;
        data.z = objeto.z;
        data.w = objeto.w;

        return data;
    }

    public SerializableCorteSave ConverterCorteSave(CorteSave objeto) {
        SerializableCorteSave data = new SerializableCorteSave();

        data.posicao = ConverterVector3(objeto.posicao);
        data.rotacao = ConverterQuaternion(objeto.rotacao);
        data.tempoDelay = objeto.tempoDelay;
        data.tempoViagem = objeto.tempoViagem;
        data.delay = objeto.delay;
        data.fixa = objeto.fixa;
        data.delayTermino = objeto.delayTermino;

        if (objeto.lookAt != null)
        {
            data.lookAt_ID = objeto.lookAt.gameObject.GetInstanceID();
        }
        else {
            data.lookAt_ID = 0;
        }

        if (objeto.posFinal != null)
        {
            data.posFinal_ID = objeto.posFinal.gameObject.GetInstanceID();
        }
        else {
            data.posFinal_ID = 0;
        }

        return data;
    }

    public CorteSave ConverterCorteSave(SerializableCorteSave objeto)
    {
        CorteSave data = new CorteSave();

        data.posicao = ConverterVector3(objeto.posicao);
        data.rotacao = ConverterQuaternion(objeto.rotacao);
        data.tempoDelay = objeto.tempoDelay;
        data.tempoViagem = objeto.tempoViagem;
        data.delay = objeto.delay;
        data.fixa = objeto.fixa;
        data.delayTermino = objeto.delayTermino;

        if (objeto.posFinal_ID != 0) {
            for (int i = 0; i < transform.childCount; i++) {
                   if (transform.GetChild(i).gameObject.GetInstanceID() == objeto.posFinal_ID) {
                    data.posFinal = transform.GetChild(i);
                    break;
                }
            }
        }

        if (objeto.lookAt_ID != 0) {
            /*
            Transform objetosPai = GameObject.FindGameObjectWithTag("Objetos").transform;
            if (objetosPai != null) {
                for (int i = 0; i < objetosPai.childCount; i++) {
                    if (objetosPai.GetChild(i).gameObject.GetInstanceID() == objeto.lookAt_ID) {
                        data.lookAt = objetosPai.GetChild(i);
                        break;
                    }
                }
            }
            */

            Transform objetosPai = GameObject.FindGameObjectWithTag("Objetos").transform;
            Transform[] objetos = objetosPai.GetComponentsInChildren<Transform>(true);
            
            for (int i = 0; i < objetos.Length; i++) {
                if (objetos[i].gameObject.GetInstanceID() == objeto.lookAt_ID)
                {
                    data.lookAt = objetos[i];
                    break;
                }
            }
        }
        
        return data;
    }

    #endregion

    #region Ohers

    private CorteSave CopiaCorte(CorteSave original)
    {
        //Debug.Log("Copiando Cortes");
        CorteSave copia = new CorteSave();
        copia.posicao = original.posicao;
        copia.rotacao = original.rotacao;
        copia.posFinal = original.posFinal;
        copia.delay = original.delay;
        copia.tempoDelay = original.tempoDelay;
        copia.fixa = original.fixa;
        copia.lookAt = original.lookAt;
        copia.tempoViagem = original.tempoViagem;
        copia.velocidadeGradativa = original.velocidadeGradativa;
        copia.delayTermino = original.delayTermino;
        copia.posicaoEmLista = original.posicaoEmLista;


        return copia;
    }

    #endregion
}
