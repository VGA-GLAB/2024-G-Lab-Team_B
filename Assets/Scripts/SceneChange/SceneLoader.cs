using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// �V�[���J�ڂ��Ǘ�����N���X
public class SceneLoader : MonoBehaviour
{

    // --------------------------------------------------
    // �ϐ��錾

    // �t�F�[�h�p�̉摜�R���|�[�l���g
    // �V�[���J�ڎ��̃t�F�[�h�C���E�A�E�g�Ɏg�p���܂�
    [Header("�t�F�[�h�p��Image")] public Image _fadeImage;

    // �t�F�[�h�ɂ����鎞�ԁi�b�j
    // �t�F�[�h�C���E�A�E�g�̑����𒲐����܂�
    [Header("�t�F�[�h�ɂ����鎞��")] public float _fadeDuration = 1f;

    // ���[�f�B���O��ʂ�UI�B
    // �񓯊��ŃV�[�������[�h����ۂɕ\�����܂�
    [Header("���[�f�B���O��ʂ�UI")] public GameObject _loadingScreen;

    // ���[�f�B���O�i����\������X���C�_�[
    // ���[�h�̐i�s�x�����o�I�Ɏ����܂�
    [Header("�i����\������X���C�_�[")] public Slider _progressBar;

    // ���[�f�B���O�i������\������e�L�X�g
    // ���[�h�̐i�s�x�𐔒l�Ŏ����܂�
    [Header("�i������\������e�L�X�g")] public Text _progressText;

    // --------------------------------------------------


    // --------------------------------------------------
    // �֐��錾


    // -----------------------
    // �O������Ăяo���֐�

    // �t�F�[�h�C���E�t�F�[�h�A�E�g�݂̂ŃV�[���J��
    // �V�[�����[�h���Ƀ��[�f�B���O��ʂ�\�����܂���
    public void LoadSceneFade(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    // �t�F�[�h�C���E�A�E�g�Ɣ񓯊����[�h��g�ݍ��킹���V�[���J��
    // ���[�f�B���O��ʂ�\�����A�񓯊��ŃV�[�������[�h���܂�
    public void LoadSceneFadeAndAsync(string sceneName)
    {
        StartCoroutine(FadeAndLoadSceneAsync(sceneName));
    }

    // -----------------------


    // -----------------------
    // �����ŌĂяo���֐�

    // �t�F�[�h�C���E�A�E�g���s���R���[�`��
    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        
        Color color = _fadeImage.color;
        
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
        
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / _fadeDuration);
       
            _fadeImage.color = new Color(color.r, color.g, color.b, newAlpha);
            
            yield return null;
        }

        _fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    // �t�F�[�h�ƃV�[�����[�h��g�ݍ��킹���R���[�`��
    IEnumerator FadeAndLoadScene(string sceneName)
    {
        // ��ʂ��Â�����t�F�[�h�A�E�g
        yield return StartCoroutine(Fade(1f, 0f)); 
        // �w�肳�ꂽ�V�[�������[�h
        SceneManager.LoadScene(sceneName); 
        // ��ʂ𖾂邭����t�F�[�h�C��
        yield return StartCoroutine(Fade(0f, 1f)); 
    }

    // �t�F�[�h�A���[�f�B���O��ʕ\���A�񓯊����[�h��g�ݍ��킹���R���[�`��
    IEnumerator FadeAndLoadSceneAsync(string sceneName)
    {
        // ��ʂ��Â�����t�F�[�h�A�E�g
        yield return StartCoroutine(Fade(1f, 0f));
        // ���[�f�B���O��ʂ�\��
        _loadingScreen.SetActive(true);
        // �V�[����񓯊��Ń��[�h
        yield return StartCoroutine(LoadSceneAsyncRoutine(sceneName));
        // ���[�f�B���O��ʂ��\���ɂ���
        _loadingScreen.SetActive(false);
        // ��ʂ𖾂邭����t�F�[�h�C��
        yield return StartCoroutine(Fade(0f, 1f)); 
    }

    // �񓯊��ŃV�[�������[�h����R���[�`��
    // �i�������X�V���Ȃ��烍�[�h���܂�
    IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // ���[�h�������Ɏ����I�ɃV�[�����A�N�e�B�u�ɂ��Ȃ��悤�ɂ��܂�
        asyncLoad.allowSceneActivation = false; 

        while (!asyncLoad.isDone)
        {
            // �i�������v�Z
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); 
            // �i������UI�ɔ��f
            // �X���C�_�[�Ői����\��
            _progressBar.value = progress; 
            // �e�L�X�g�Ői�����𐔒l�\��
            _progressText.text = (progress * 100f).ToString("0") + "%"; 

            // ���[�h���قڊ���������V�[�����A�N�e�B�u�ɂ���
            if (asyncLoad.progress >= 0.9f)
            {
                // ���[�h������ɃV�[�����A�N�e�B�u��
                asyncLoad.allowSceneActivation = true;
            }

            // ���̃t���[���܂őҋ@
            yield return null; 
        }
    }

    // -----------------------

    // --------------------------------------------------
}
