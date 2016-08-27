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
public class CorteSave
{
    [HideInInspector]
    public Vector3 posicao;
    [HideInInspector]
    public Quaternion rotacao;
    public bool delay = false, fixa = true, velocidadeGradativa = false;
    public float tempoDelay = 0, tempoViagem = 0, delayTermino = 0;
    public Transform lookAt = null, posFinal = null;

}

[Serializable]
public class EdicaoCorte
{
    public int indiceEdicao = 0;
    public bool abrirCorte = false, salvarEdicao = false, zerarEdicao = false, setCameraCorte = false, deletarCorte = false, utilizarCameraAtual = false;
    public CorteSave corteSelecionado = new CorteSave();
    [HideInInspector]
    public bool aberto = false;
    [HideInInspector]
    public int corteAberto = 0;
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

    #region Variaveis
    [Header("Configurações de Backup")]
    [Tooltip("Salva a lista de cortes num caminho específico")]
    public bool saveBackup = false;
    [Tooltip("Carrega a lista de cortes (se houver alguma)")]
    public bool loadBackup = false;


    [Header("Configurações de camera atual")]
    [Tooltip("Salva a posição inicial da camera")]
    public bool setPosInicial = false;
    [Tooltip("Define a posição da camera inicial salva")]
    public bool voltaCameraInicial = false;

    [Header("Configurações do corte atual")]
    public bool addCorte = false;
    private bool deletarUltimo = false;
    public bool listarCortes = false; public bool deletarTudo = false;
    public bool zerarObjeto = false; public bool setCorteAnterior = false;


    private int indicePos = 0;

    [SerializeField]
    [HideInInspector]
    public int numeroPos = 0;


    //[Header("Visualização do corte")]
    public CorteSave corteAtual = new CorteSave();

    [Header("Edições de Cortes")]
    public EdicaoCorte edicaoCorte = new EdicaoCorte();
    public AdicaoCorte insertCorte = new AdicaoCorte();


    [SerializeField]
    [HideInInspector]
    public List<CorteSave> posicoesCorte = new List<CorteSave>();

    [SerializeField]
    [HideInInspector]
    private CorteSave corteInicial = new CorteSave();


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
        }
        else if (deletarUltimo)
        {
            deletarUltimo = false;
            DeletarUltimaPos();
        }
        else if (listarCortes)
        {
            listarCortes = false;
            ListarCortesTxt();
        }
        else if (deletarTudo)
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
        }
        else if (setCorteAnterior)
        {
            setCorteAnterior = false;
            SetCameraEmAnterior();
        }
        else if (setPosInicial)
        {
            setPosInicial = false;
            corteInicial.rotacao = Camera.main.transform.rotation;
            corteInicial.posicao = Camera.main.transform.position;
        }
        else if (voltaCameraInicial)
        {
            voltaCameraInicial = false;
            Camera.main.transform.position = corteInicial.posicao;
            Camera.main.transform.rotation = corteInicial.rotacao;
        }
        else if (saveBackup)
        {
            saveBackup = false;
            Save();
        }
        else if (loadBackup)
        {
            loadBackup = false;
            Load();
        }

        #endregion

        #region InputEdição
        if (edicaoCorte.abrirCorte)
        {
            if (edicaoCorte.indiceEdicao >= 0 && edicaoCorte.indiceEdicao < posicoesCorte.Count)
            {
                edicaoCorte.aberto = true;
                //Debug.Log("Abriu");

                edicaoCorte.corteSelecionado = CopiaCorte(posicoesCorte[edicaoCorte.indiceEdicao]);
            }
            else if (EditorUtility.DisplayDialog("Indisponível", "Este corte não existe. Consulte a lista de cortes.", "Ok"))
            {
                edicaoCorte.indiceEdicao = posicoesCorte.Count - 1;
            }

            edicaoCorte.abrirCorte = false;
            edicaoCorte.corteAberto = edicaoCorte.indiceEdicao;
        }
        else if (edicaoCorte.zerarEdicao)
        {
            edicaoCorte.utilizarCameraAtual = false;
            edicaoCorte.aberto = false;
            edicaoCorte.corteSelecionado = new CorteSave();
            edicaoCorte.zerarEdicao = false;
        }
        else if (edicaoCorte.salvarEdicao)
        {
            if (edicaoCorte.aberto)
            {
                if (EditorUtility.DisplayDialog("Confirmar save", "Deseja sobrepor este corte pelo da lista anterior?", "Sim", "Não"))
                {
                    if (edicaoCorte.utilizarCameraAtual) {
                        edicaoCorte.corteSelecionado.posicao = Camera.main.gameObject.transform.position;
                        edicaoCorte.corteSelecionado.rotacao = Camera.main.gameObject.transform.rotation;
                    }
                    posicoesCorte[edicaoCorte.indiceEdicao] = CopiaCorte(edicaoCorte.corteSelecionado);
                }
            }
            else if (EditorUtility.DisplayDialog("Abrir corte", "Abra um corte antes de salvar.", "Ok"))
            {
            }

            edicaoCorte.salvarEdicao = false;
        }
        else if (edicaoCorte.setCameraCorte)
        {
            if (edicaoCorte.aberto)
            {
                Camera.main.gameObject.transform.position = edicaoCorte.corteSelecionado.posicao;
                Camera.main.gameObject.transform.rotation = edicaoCorte.corteSelecionado.rotacao;
            }
            else if (EditorUtility.DisplayDialog("Abrir corte", "Abra um corte antes de configurar-lo.", "Ok"))
            {
            }

            edicaoCorte.setCameraCorte = false;
        }
        else if (edicaoCorte.deletarCorte)
        {
            if (edicaoCorte.aberto)
            {
                if (EditorUtility.DisplayDialog("Deletar corte", "Deseja mesmo deletar este corte? Os demais cortes serão realocados para suas novas posições.", "Sim", "Não"))
                {
                    posicoesCorte.RemoveAt(edicaoCorte.indiceEdicao);
                }
            }

            edicaoCorte.deletarCorte = false;
        }

        if (edicaoCorte.aberto == true)
        {
            if (edicaoCorte.indiceEdicao != edicaoCorte.corteAberto)
            {
                if (EditorUtility.DisplayDialog("Fechar corte.", "Feche o corte atual para alterar o indice.", "Ok"))
                {
                }
            }

            edicaoCorte.indiceEdicao = edicaoCorte.corteAberto;
        }
        #endregion

        #region InputAddCorte
        if (insertCorte.addCorte)
        {
            insertCorte.addCorte = false;
            InsertCorte();
        }
        else if (insertCorte.zerarObjeto) {
            insertCorte.zerarObjeto = false;
            insertCorte.corteSelecionado = new CorteSave();
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

    private void SetCameraEmAnterior()
    {
#if UNITY_EDITOR
        if (posicoesCorte.Count > 0)
        {
            Camera.main.gameObject.transform.position = posicoesCorte[numeroPos].posicao;
            Camera.main.gameObject.transform.rotation = posicoesCorte[numeroPos].rotacao;
            Debug.Log("Setado na ultima posição");
        }
        else
        {
            Debug.Log("Não há ultima posição");
        }
#endif
    }

    private void ListarCortesTxt()
    {
#if UNITY_EDITOR
        int temp = 0;
        string lines = "";
        foreach (CorteSave a in posicoesCorte)
        {
            lines += "Indice: " + temp + "\r\n";
            lines += "Pos: (" + a.posicao.x + ", " + a.posicao.y + ", " + a.posicao.z + ")" + "\r\n";
            lines += "Delay: " + a.delay + "\r\n";
            lines += "Tempo delay: " + a.tempoDelay + "\r\n";
            lines += "Tempo Viagem: " + a.tempoViagem + "\r\n";
            lines += "LookAt: " + a.lookAt + "\r\n";
            lines += "PosFinal: " + a.posFinal + "\r\n";

            lines += "\r\n" + "\r\n" + "\r\n" + "\r\n";
            temp++;
        }

        if (temp == 0)
        {
            lines = "Nenhum corte listado";
            Debug.Log(lines);
        }
        else
        {
            try
            {
                // Write the string to a file.
                System.IO.StreamWriter file = new System.IO.StreamWriter(diretorioListagem);
                file.WriteLine(lines);

                file.Close();
                Debug.Log("Cortes listados com sucesso.");
            }
            catch
            {
                Debug.LogError("Cortes não puderam ser listados");
            }
        }



#endif
    }

    private void DeletarUltimaPos()
    {
#if UNITY_EDITOR

        int indiceDel = posicoesCorte.Count - 1;
        if (indiceDel >= 0)
        {
            posicoesCorte.RemoveAt(indiceDel);
            numeroPos = 0;
            Debug.Log("Posicao deletada com sucesso." + "\nIndice: " + indiceDel);
            ZerarObjetoAtual();
        }
        else
        {
            Debug.Log("Não é possivel deletar o ultimo corte");
        }

#endif
    }

    private void DeletarListaCompleta()
    {
        posicoesCorte.Clear();
        numeroPos = 0;
        Debug.Log("Lista deletada");
        ZerarObjetoAtual();
    }

    private void AddNovaPos()
    {
#if UNITY_EDITOR
        if (Camera.main != null)
        {
            numeroPos++;
            CorteSave objetoTemp = corteAtual;
            objetoTemp.posicao = Camera.main.transform.position;
            objetoTemp.rotacao = Camera.main.transform.rotation;


            posicoesCorte.Add(objetoTemp);
            Debug.Log("Posicao Adicionada com sucesso." + "\nIndice: " + (numeroPos - 1).ToString() + "\nPos: " + objetoTemp.posicao + "\nRot: " + objetoTemp.rotacao);
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

    #region f_Adicao

    private void InsertCorte() {
#if UNITY_EDITOR
        if (insertCorte.novoIndice >= 0)
        {
            if (insertCorte.novoIndice < posicoesCorte.Count)
            {
                if (EditorUtility.DisplayDialog("Adição de corte", "Este corte será adicionado no indice " + insertCorte.novoIndice.ToString() +
                    ". Os cortes serão realocados para uma casa depois"
                    , "Adicionar", "Não adicionar"))
                {
                    insertCorte.corteSelecionado.posicao = Camera.main.transform.position;
                    insertCorte.corteSelecionado.rotacao = Camera.main.transform.rotation;

                    numeroPos++;
                    posicoesCorte.Insert(insertCorte.novoIndice, insertCorte.corteSelecionado);
                }
            }
            else
            {
                if (insertCorte.novoIndice > 0)
                {
                    insertCorte.novoIndice = posicoesCorte.Count - 1;
                }
                else
                {
                    insertCorte.novoIndice = 0;
                }

                if (EditorUtility.DisplayDialog("Valor incorreto", "Este corte não pode ser adicionado na ultima posição da lista. Adicione-o através do editor original.", "Ok"))
                {
                }
            }
        }
        else
        {
            insertCorte.novoIndice = 0;
            if (EditorUtility.DisplayDialog("Valor incorreto", "Este corte não pode ser adicionado. Adicione um indice válido.", "Ok"))
            {
                insertCorte.novoIndice = 0;
            }
        }
#endif
    }

    #endregion

    #region f_Backup
    private void Save()
    {
#if UNITY_EDITOR
        List<SerializableCorteSave> data = new List<SerializableCorteSave>();


        for (int i = 0; i < posicoesCorte.Count; i++)
        {
            SerializableCorteSave corte = ConverterCorteSave(posicoesCorte[i]);
            data.Add(corte);
        }


        try
        {
            if (File.Exists(diretorioSave + "corte.bkup"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(diretorioSave + "corte.bkup");
                if (EditorUtility.DisplayDialog("Confirmar save", "Já existe um arquivo de Backup, deseja sobrepor o arquivo?", "Sim", "Não"))
                {
                    bf.Serialize(file, data);
                    file.Close();
                    Debug.Log("Lista salva com sucesso.");
                }
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(diretorioSave + "corte.bkup");
                bf.Serialize(file, data);
                file.Close();
                Debug.Log("Lista salva com sucesso.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

#endif

    }

    private void Load()
    {
#if UNITY_EDITOR
        if (posicoesCorte.Count != 0)
        {
            if (EditorUtility.DisplayDialog("Deletar Cortes", "Você deseja excluir os cortes que você tem carregado no Inspector? ", "Deletar e carregar Backup!", "Não Deletar"))
            {
            }
            else
            {
                return;
            }
        }


        if (File.Exists(diretorioSave + "corte.bkup"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(diretorioSave + "corte.bkup", FileMode.Open);
            List<SerializableCorteSave> data = (List<SerializableCorteSave>)bf.Deserialize(file);
            file.Close();

            posicoesCorte.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                posicoesCorte.Add(ConverterCorteSave(data[i]));
            }

            numeroPos = posicoesCorte.Count - 1;
            Debug.Log("Loading realizado com sucesso");
        }
        else if (EditorUtility.DisplayDialog("Nenhum backup", "Nenhum Backup foi encontrado. Verifique pelo arquivo no diretório: " + diretorioSave, "Ok"))
        {
        }
#endif
    }
    #endregion

    #region GettersSetters
    public CorteSave GetCorte(int indice) {
        //Debug.Log("Indice: " + indice + "\nCount: " + posicoesCorte.Count);
        if (posicoesCorte.Count > indice)
        {
            return posicoesCorte[indice];
        }
        else {
            return null;
        }
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
        Debug.Log("Copiando Cortes");
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

        return copia;
    }

    #endregion
}




