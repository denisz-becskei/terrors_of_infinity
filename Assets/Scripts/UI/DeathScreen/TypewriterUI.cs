using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypewriterUI : MonoBehaviour
{
    private TMP_Text _tmpProText;
    private string writer;
    private AudioSource audioSource;

    private bool animating = false;
    private bool skip = false;

    [SerializeField] float delayBeforeStart = 0f;
    [SerializeField] float timeBtwChars = 0.1f;
    [SerializeField] string leadingChar = "";
    [SerializeField] bool leadingCharBeforeDelay = false;

    [SerializeField] GameObject skullScreen;
    [SerializeField] GameObject continueButton;

    // Use this for initialization
    void Start()
    {
        _tmpProText = GetComponent<TMP_Text>()!;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && animating)
        {
            skip = true;
        }
    }

    public void StartTypewrite()
    {
        if (_tmpProText != null)
        {
            writer = _tmpProText.text;
            _tmpProText.text = "";
        }

        StartCoroutine(TypeWriterTMP());
    }

    IEnumerator TypeWriterTMP()
    {
        _tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";
        animating = true;
        bool successfullySkipped = false;
        yield return new WaitForSeconds(delayBeforeStart);
        skullScreen.GetComponent<DestroySkull>().Destroy(GetMaxIndex());

        foreach (char c in writer)
        {
            if (_tmpProText.text.Length > 0)
            {
                _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
            }
            _tmpProText.text += c;
            _tmpProText.text += leadingChar;

            if(skip)
            {
                skip = false;
                successfullySkipped = true;
                _tmpProText.text = writer;
                break;
            }

            audioSource.Play();
            yield return new WaitForSeconds(timeBtwChars);
        }
        continueButton.GetComponent<ContinueButtonController>().Animate();
        animating = false;
        if (leadingChar != "" && !successfullySkipped)
        {
            _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
        }
    }

    private uint GetMaxIndex()
    {
        Transform[] transforms = skullScreen.GetComponentsInChildren<Transform>();
        return (uint)(transforms.Length - 1);
    }
}