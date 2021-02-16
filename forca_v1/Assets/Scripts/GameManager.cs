using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int numTentativas; // Numero de tentativas validas da rodada
    private int maxNumTentativas; // Maximo numero de tentativas para Forca ou Salvaçao
    int score = 0; // Pontuaçao
    public GameObject letra; //prefab de letra
    public GameObject centro; // ojeto de texto que indica o centro da tela

    private string palavraOculta = ""; //palavra a ser advinhada
    //private string[] palavrasOcultas = new string[] { "carro", "elefante", "futebol" }; // array de palavras ocultas
    private int tamanhoPalavraOculta; // tamanho da palavra oculta
    char[] letrasOcultas; // letras da plavra a ser descoberta
    bool[] letrasDescobertas; // indicador de quais letras foram descobertas

    // Start is called before the first frame update
    void Start()
    {
        centro = GameObject.Find("centroDaTela");
        InitGame();
        InitLetras();
        numTentativas = 0;
        maxNumTentativas = (int)(palavraOculta.Length*1.5f);
        UpdateNumTentativas();
        UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
        checkTeclado();
    }

    void InitLetras()
    {
        int numLetras = tamanhoPalavraOculta;
        for(int i = 0; i < numLetras; i++)
        {
            Vector3 novaPosicao;
            novaPosicao = new Vector3(centro.transform.position.x + ((i-numLetras/2.0f)*80), centro.transform.position.y, centro.transform.position.z);
            GameObject l = (GameObject)Instantiate(letra, novaPosicao, Quaternion.identity);
            l.name = "letra" + (i + 1); // nomeia na hierarquia a object com letra i-ésima, i=1,...
            l.transform.SetParent(GameObject.Find("Canvas").transform); // posicionar como filho do Game Object
        }
    }

    void InitGame()
    {
        //palavraOculta = "Elefante"; //definiçao da palavra a ser descoberta => usando no "Lab1 - Parte A"
        //int numeroAleatorio = Random.Range(0, palavrasOcultas.Length);
        //palavraOculta = palavrasOcultas[numeroAleatorio];

        palavraOculta = PegaUmaPalavraDoArquivo(); // recebendo a palavra oculta da função que sorteia palavras ocultas contidas em um arquivo txt

        tamanhoPalavraOculta = palavraOculta.Length; //determinando o numero de letras da palavra oculta
        palavraOculta = palavraOculta.ToUpper(); // transformando a palavra em maiusculas
        letrasOcultas = new char[tamanhoPalavraOculta]; // criando array de char das letras da palavra
        letrasDescobertas = new bool[tamanhoPalavraOculta]; // criando array de bool das letras da palavra
        letrasOcultas = palavraOculta.ToCharArray(); // copiando palavra no array de letras

    }

    void checkTeclado()
    {
        if (Input.anyKeyDown)
        {
            char letraTeclada = Input.inputString.ToCharArray()[0];
            int letraTecladaComoInt = System.Convert.ToInt32(letraTeclada);
            if(letraTecladaComoInt>=97 && letraTecladaComoInt <= 122)
            {
                numTentativas++;
                UpdateNumTentativas();
                if(numTentativas >= maxNumTentativas)
                {
                    SceneManager.LoadScene("Lab1_forca");
                }
                for(int i = 0; i < tamanhoPalavraOculta; i++)
                {
                    if (!letrasDescobertas[i])
                    {
                        letraTeclada = System.Char.ToUpper(letraTeclada);
                        if(letrasOcultas[i] == letraTeclada)
                        {
                            letrasDescobertas[i] = true;
                            GameObject.Find("letra" + (i + 1)).GetComponent<Text>().text = letraTeclada.ToString();
                            score = PlayerPrefs.GetInt("score");
                            score++;
                            PlayerPrefs.SetInt("score", score);
                            UpdateScore();
                            VerificaSePalavraDescoberta();
                        }
                    }
                }
            }
        }
    }

    void UpdateNumTentativas()
    {
        GameObject.Find("numTentativas").GetComponent<Text>().text = numTentativas + "|" + maxNumTentativas;
    }

    void UpdateScore()
    {
        GameObject.Find("scoreUI").GetComponent<Text>().text = "Score: " + score;
    }

    void VerificaSePalavraDescoberta()
    {
        bool condicao = true;
        for(int i = 0; i < tamanhoPalavraOculta; i++)
        {
            condicao = condicao && letrasDescobertas[i];
        }
        if (condicao)
        {
            PlayerPrefs.SetString("ultimaPalavraOculta",palavraOculta);
            SceneManager.LoadScene("Lab1_salvo");
        }
    }

    string PegaUmaPalavraDoArquivo()
    {
        TextAsset t1 = (TextAsset)Resources.Load("palavras",typeof(TextAsset));
        string s = t1.text;
        string[] palavras = s.Split(' ');
        int palavraAleatoria = Random.Range(0,palavras.Length + 1);
        return palavras[palavraAleatoria];
    }
}
