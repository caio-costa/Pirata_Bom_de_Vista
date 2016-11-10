using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GruposNPCBehaviour : MonoBehaviour {

    public bool ativarComDiferenca = false;
    public float diferenca = 0;
    public AnimationClip[] animacoes;
    public List<PosObjetivos> listaPosicoes = new List<PosObjetivos>();

    private Transform myTransform;

    // Use this for initialization
    void Awake () {
        myTransform = this.transform;
        if (ativarComDiferenca) {
            if (diferenca == 0) {
                diferenca = 0.5f;
            }
        }
	}



    void OnEnable()
    {
        //Debug.Log("Novo Objetivo, mudar de pos");
        ObjetivosController.eventoNovoObjetivo += ConfiguraPosObjetivo;
    }

    void OnDisable()
    {
        ObjetivosController.eventoNovoObjetivo -= ConfiguraPosObjetivo;
    }


    public void ConfiguraPosObjetivo()
    {
        //Debug.Log("Novo Objetivo, mudar de pos");
        bool fazParte = false;

        if (CheckSeFazPartePosicaoObjetivo())
        {
            fazParte = true;
        }
        else {
            fazParte = false;
        }

        //Debug.Log(fazParte);

        MudaTodosOsFilhos(fazParte);
    }

    public void MudaTodosOsFilhos(bool temp) {
        if (!ativarComDiferenca)
        {
            for (int i = 0; i < myTransform.childCount; i++)
            {
                myTransform.GetChild(i).gameObject.SetActive(temp);
            }
        }
        else if (temp == true)
        {
            StartCoroutine(AtivarComDelay());
        }
        else if (temp == false) {
            //Debug.Log("Era pra desativar");
            for (int i = 0; i < myTransform.childCount; i++)
            {
                myTransform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }


    public IEnumerator AtivarComDelay() {

        for (int i = 0; i < myTransform.childCount; i++)
        {
            myTransform.GetChild(i).gameObject.SetActive(true);
            if (myTransform.GetChild(i).gameObject.CompareTag("Ignorar")) {
                continue;
            }

            Animator animator = myTransform.GetChild(i).GetChild(0).GetChild(1).gameObject.GetComponent<Animator>();
            if (animator != null) {
                RuntimeAnimatorController myController = animator.runtimeAnimatorController;

                AnimatorOverrideController anim_OverrideController = new AnimatorOverrideController();
                anim_OverrideController.runtimeAnimatorController = myController;

                animator.runtimeAnimatorController = anim_OverrideController;
                animator.SetBool("Bebado", true);

                bool mudaAnim = false;
                int random = Random.Range(0, 33);

                for (int a = 0; a < random; a++)
                {
                    mudaAnim = !mudaAnim;
                }

                mudaAnim = true;
                if (mudaAnim)
                {

                    AnimationClip clipe = animacoes[Random.Range(0, animacoes.Length)];


                    if (clipe != null)
                    {
                        //Debug.Log(clipe.name);
                        anim_OverrideController[""] = clipe;
                    }
                }

                yield return new WaitForSeconds(diferenca);

            }
            yield return null;
        }
    }

    public bool CheckSeFazPartePosicaoObjetivo()
    {
        FaseAtual faseAtual = ObjetivosController.instance.GetFaseAtual();

        for (int i = 0; i < listaPosicoes.Count; i++)
        {
            if (listaPosicoes[i].faseCorrespondente.fase == faseAtual.fase && listaPosicoes[i].faseCorrespondente.indice == faseAtual.indice)
            {
                //Debug.Log("Faz parte");
                return true;
            }
        }

        return false;
    }


}

[System.Serializable]
public class PosObjetivos
{
    public FaseAtual faseCorrespondente = new FaseAtual();
}