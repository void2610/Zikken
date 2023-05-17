using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

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
    private Text nameText;
    [SerializeField]
    private GameObject nameField;
    [SerializeField]
    private GameObject enterButton;

    // System.IO
    private StreamWriter sw;
    //アプリケーションのパス
    private string path;

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
    private int[] image = new int[9];
    private int imageIndex = -1;

    public void OnClickEnterButton(){
        state = State.Introduction;
        name = nameText.text;
        if(name == ""){
            name = "none";
        }
        nameField.gameObject.SetActive(false);
        enterButton.gameObject.SetActive(false);
        introductionText.gameObject.SetActive(true);

    }

    private void SetImage(){
        Debug.Log(imageIndex);
        if(Random.value < 0.5f)
        {
            upImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Right/" + image[imageIndex]);
            downImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Left/" + image[imageIndex]);
        }
        else
        {
            downImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Right/" + image[imageIndex]);
            upImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Left/" + image[imageIndex]);
        }
    }

    private void SaveData(){
        float time = (endTime - startTime);
        if(key == "space"){
            key = "down";
        }
        else if(key == "enter"){
            key = "up";
        }
        string[] s1 = { time.ToString(), key};
        string s2 = string.Join(",", s1);
        SaveToFile(s2);
    }

    private void SaveToFile(string content)
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, name + ".csv");
        using (var writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(content);
            Debug.Log("Saved");
        }
    }

    void Start()
    {
        //image配列に1~9の数字を被りなく、ランダムに代入
        for (int i = 0; i < image.Length; i++)
        {
            image[i] = Random.Range(1, 10);
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
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    state = State.Testing;
                    upImage.gameObject.SetActive(true);
                    downImage.gameObject.SetActive(true);
                    introductionText.gameObject.SetActive(false);
                }
                break;
            case State.Testing:
                if(Input.GetKeyDown(KeyCode.Space)){
                    key = "space";
                }
                else if(Input.GetKeyDown(KeyCode.Return)){
                    key = "enter";
                }

                //spaceかenterが押されたら画像を更新
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {

                    //imageIndexが9になったら終了
                    if (imageIndex == 9)
                    {
                        upImage.gameObject.SetActive(false);
                        downImage.gameObject.SetActive(false);
                        introductionText.gameObject.SetActive(true);
                        state = State.End;
                        imageIndex = 0;
                        break;
                    }
                    else{
                        if(startTime == 0){
                            startTime = Time.time;
                        }
                        else{
                            endTime = Time.time;
                            SetImage();
                            SaveData();
                            startTime = Time.time;
                        }
                    imageIndex++;
                    }
                }
                break;
            case State.End:
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
