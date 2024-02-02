using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<GameObject> baraja, cartasEnManoPlayer;
    [SerializeField] Transform manoPlayer,manoMesa;
    [SerializeField] Button pedirCartaBTN,pasarBTN;
    [SerializeField] TMP_Text textoCuenta,textoCuentaMesa,feedbackText;
    private int cuenta,cuentaMesa;
    private bool pasar;
    private void Start()
    {
        pasarBTN.onClick.AddListener(() => pasar = true);
        pedirCartaBTN.onClick.AddListener(() =>
        PonerCartaEnMesa(baraja, manoPlayer.transform.position));
        pedirCartaBTN.interactable = false;
        StartCoroutine(GameLoop(0));
    }
    IEnumerator GameLoop(int apuesta)
    {
        yield return new WaitForSeconds(1);
        baraja = DesordenarBaraja(baraja);
        PonerCartaEnMesa(baraja,manoPlayer.transform.position); yield return new WaitForSeconds(.35f); PonerCartaEnMesa(baraja,manoPlayer.transform.position);
        pedirCartaBTN.interactable =true;

        while(cuenta <= 21 && !pasar )
        {
            pedirCartaBTN.interactable = true;
            yield return new WaitForEndOfFrame();
        }
        if (cuenta > 21) { feedbackText.text = $"Tu tienes {cuenta}, Has perdido!";
            yield return new WaitForSeconds(3); SceneManager.LoadScene(0);
        }
        else if(cuenta == 21)
        {
            feedbackText.text = $"Tu tienes: {cuenta}!, Tu ganas!";
            yield return new WaitForSeconds(3); SceneManager.LoadScene(0);
        }
        else
        {
            while(cuentaMesa < 21 || cuentaMesa < cuenta)
            {
                PonerCartaEnMesa(baraja, manoMesa.position, false);
                yield return new WaitForSeconds(.75f);
            }
            if(cuentaMesa > cuenta && cuentaMesa <=21)
            {
                feedbackText.text = "gana la mesa";
            }
            else
            {
                feedbackText.text = "Ganas tu!";
            }
            yield return new WaitForSeconds(3); SceneManager.LoadScene(0);
        }


    }
    public void PonerCartaEnMesa(List<GameObject> baraja,Vector2 pos,bool esPlayer=true)
    {
        var carta = baraja[baraja.Count - 1];
        baraja.Remove(carta);
        carta.transform.DOMove(pos, .5f).OnComplete(() =>
        {
          int valor = 0;
            string cVal = carta.name.Split('_')[1];
              if (cVal == "J" || cVal == "Q" || cVal == "K"||cVal == "A" ) valor = 10;
              else valor = int.Parse(cVal);
                if(!esPlayer)
                {
                    cuentaMesa += valor;
                     carta.transform.parent = manoMesa.transform;
                     textoCuentaMesa.text = $"Mesa : {cuentaMesa}";
                    return;
                }
             cartasEnManoPlayer.Add(carta);
            carta.transform.parent = manoPlayer;
            cuenta += valor;
            textoCuenta.text = $"Tu tienes: {cuenta}";
        });
        
    }
    public List<GameObject> DesordenarBaraja(List<GameObject> cartas)
    {
        var tmpBaraja = new List<GameObject>();
        while (cartas.Count > 0)
        {
            int r = Random.Range(0, cartas.Count);
            tmpBaraja.Add(cartas[r]);
            cartas.RemoveAt(r);
        }
        return tmpBaraja;
    }



    [Header("Parte de generacion")]
    [SerializeField] GameObject prefabMoldeCarta;
    [SerializeField] List<Sprite> spritesCartas;

    [ContextMenu("create cards")]
    public void CreateCards()
    {
        Vector3 StartPos = Vector3.zero;
        foreach (var sprite in spritesCartas)
        {
            var c = Instantiate(prefabMoldeCarta,StartPos,Quaternion.identity);
            c.GetComponent<Image>().sprite = sprite;
             c.name = sprite.name;
            StartPos.z+=1;
        }
    }
}
