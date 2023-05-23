using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using TMPro;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private Image upImage;
    [SerializeField]
    private Image downImage;
    [SerializeField]
    private Text introductionText;
    [SerializeField]
    private Text outroductionText;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private GameObject nameField;
    [SerializeField]
    private GameObject enterButton;

    private string upData = "";
    private string downData = "";

    // System.IO
    private StreamWriter sw;
    //アプリケーションのパス
    private string path;

    bool stateEnter = true;

    public enum State
    {
        EnterName,
        Introduction,
        Testing,
        End
    }

    private float startTime = 0;
    private float endTime = 0;

    private State state = State.EnterName;
    private string key = "none";
    private string name = "none";

    //要素数9のint配列
    private int[] image = new int[8];
    private int imageIndex = 0;

    private void ChangeState(State s){
        state = s;
        stateEnter = true;
    }

    public void OnClickEnterButton(){
        state = State.Introduction;
        name = nameText.text;
        if(name == ""){
            name = "none";
        }

        //name + ".csv"というファイルが存在したら、番号を追加する
        int n = 1;
        string originalName = name;
        while(File.Exists(Path.Combine(Application.streamingAssetsPath, name + ".csv"))){
            name = originalName + n.ToString();
            n++;
        }

        nameField.gameObject.SetActive(false);
        enterButton.gameObject.SetActive(false);
        introductionText.gameObject.SetActive(true);

    }

    private void SetImage(){
        Debug.Log(image[imageIndex]);
        int n = image[imageIndex];
        if(Random.value < 0.5f)
        {
            upImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Right/" + n);
            downImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Left/" + n);
            upData = "right";
            downData = "left";
        }
        else
        {
            downImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Right/" + n);
            upImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Left/" + n);
            upData = "left";
            downData = "right";
        }
    }

    private void SaveData(){
        float time = (endTime - startTime);
        if(key == "space"){
            key = downData;
        }
        else if(key == "enter"){
            key = upData;
        }
        string[] s1 = { time.ToString(), key, image[imageIndex-1].ToString()};

        using (var writer = new StreamWriter(Path.Combine(Application.streamingAssetsPath, name + ".csv"), true))
        {
            writer.WriteLine(string.Join(",", s1));
            Debug.Log("Saved");
        }
    }

    void Start()
    {
        //image配列に1~9の数字を被りなく、ランダムに代入
        for (int i = 0; i < image.Length; i++)
        {
            image[i] = Random.Range(1, 9);
            for (int j = 0; j < i; j++)
            {
                if (image[i] == image[j])
                {
                    i--;
                    break;
                }
            }
        }

        upImage.gameObject.SetActive(false);
        downImage.gameObject.SetActive(false);
        introductionText.gameObject.SetActive(false);
        outroductionText.gameObject.SetActive(false);
        nameField.gameObject.SetActive(true);
        enterButton.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Introduction:
                if(stateEnter){
                    stateEnter = false;
                    startTime = Time.time;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState(State.Testing);
                    upImage.gameObject.SetActive(true);
                    downImage.gameObject.SetActive(true);
                    introductionText.gameObject.SetActive(false);
                }
                break;
            case State.Testing:
                if(stateEnter){
                    stateEnter = false;

                    SetImage();
                    imageIndex++;
                    startTime = Time.time;
                }

                if(Input.GetKeyDown(KeyCode.Space)){
                    key = "space";
                }
                else if(Input.GetKeyDown(KeyCode.Return)){
                    key = "enter";
                }

                //spaceかenterが押されたら画像を更新
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    endTime = Time.time;
                    SaveData();

                    //imageIndexが9になったら終了
                    if (imageIndex >= 8)
                    {
                        ChangeState(State.End);
                        break;
                    }
                    else{
                        SetImage();
                        imageIndex++;
                    }
                    startTime = Time.time;
                }
                break;
            case State.End:
                if(stateEnter){
                    stateEnter = false;
                }
                upImage.gameObject.SetActive(false);
                downImage.gameObject.SetActive(false);
                introductionText.gameObject.SetActive(false);
                outroductionText.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
