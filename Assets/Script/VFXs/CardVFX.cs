using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardVFX : MonoBehaviour
{
    [Header("Glow")]
    public Image glowImage;

    [Header("Shimmer")]
    public Image shimmerImage;

    [Header("Hint Settings")]
    public float hintDelay = 10f;

    private Coroutine glowCoroutine;
    private Coroutine shimmerCoroutine;
    private Coroutine hintCoroutine;

    // ── FIX: isPlayable is now ONLY true when this card
    //    is actively in a valid player-turn hint cycle.
    //    It is set to false the moment the turn ends OR
    //    the card is played, before any coroutine is stopped.
    private bool isPlayable = false;

    void Start()
    {
        ResetGlow();
        ResetShimmer();
    }

    void ResetGlow()
    {
        if (glowImage != null)
        {
            Color c = glowImage.color;
            c.a = 0f;
            glowImage.color = c;
            glowImage.raycastTarget = false;
        }
    }

    void ResetShimmer()
    {
        if (shimmerImage != null)
        {
            Color c = shimmerImage.color;
            c.a = 0f;
            shimmerImage.color = c;
            shimmerImage.raycastTarget = false;
        }
    }

    // ── Playable Glow ────────────────────────────────────

    public void SetPlayable(bool playable)
    {
        // ── FIX: Always set isPlayable FIRST so any running
        //    PulseGlow() loop sees the new value immediately
        //    and exits cleanly on its next iteration.
        isPlayable = playable;

        // Stop both coroutines AFTER flipping the flag
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }

        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
            glowCoroutine = null;
        }

        if (playable)
        {
            // Start fresh hint countdown
            hintCoroutine = StartCoroutine(StartHintAfterDelay());
        }
        else
        {
            // Immediately fade glow out
            glowCoroutine = StartCoroutine(FadeGlow(0f));
        }
    }

    IEnumerator StartHintAfterDelay()
    {
        yield return new WaitForSeconds(hintDelay);

        // Only pulse if STILL playable (turn hasn't ended)
        if (isPlayable)
            glowCoroutine = StartCoroutine(PulseGlow());
    }

    IEnumerator PulseGlow()
    {
        if (glowImage == null) yield break;

        while (isPlayable)
        {
            // Fade in
            float elapsed = 0f;
            while (elapsed < 0.5f && isPlayable)
            {
                elapsed += Time.deltaTime;
                Color c = glowImage.color;
                c.a = Mathf.Lerp(0f, 0.85f, elapsed / 0.5f);
                glowImage.color = c;
                yield return null;
            }

            if (!isPlayable) break;
            yield return new WaitForSeconds(0.2f);
            if (!isPlayable) break;

            // Fade out
            elapsed = 0f;
            while (elapsed < 0.5f && isPlayable)
            {
                elapsed += Time.deltaTime;
                Color c = glowImage.color;
                c.a = Mathf.Lerp(0.85f, 0.1f, elapsed / 0.5f);
                glowImage.color = c;
                yield return null;
            }

            if (!isPlayable) break;
            yield return new WaitForSeconds(0.2f);
        }

        // Always fade out cleanly when loop exits
        yield return StartCoroutine(FadeGlow(0f));
    }

    IEnumerator FadeGlow(float targetAlpha)
    {
        if (glowImage == null) yield break;

        float startAlpha = glowImage.color.a;
        float elapsed = 0f;

        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            Color c = glowImage.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / 0.3f);
            glowImage.color = c;
            yield return null;
        }

        Color final = glowImage.color;
        final.a = targetAlpha;
        glowImage.color = final;
    }

    // ── Shimmer ──────────────────────────────────────────

    public void PlayShimmer()
    {
        if (shimmerCoroutine != null)
            StopCoroutine(shimmerCoroutine);
        shimmerCoroutine = StartCoroutine(DoShimmer());
    }

    IEnumerator DoShimmer()
    {
        if (shimmerImage == null) yield break;

        RectTransform rt = shimmerImage.GetComponent<RectTransform>();
        if (rt != null)
            rt.anchoredPosition = new Vector2(-100f, 0f);

        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Color c = shimmerImage.color;
            c.a = t < 0.5f
                ? Mathf.Lerp(0f, 0.6f, t * 2f)
                : Mathf.Lerp(0.6f, 0f, (t - 0.5f) * 2f);
            shimmerImage.color = c;

            if (rt != null)
                rt.anchoredPosition =
                    new Vector2(Mathf.Lerp(-100f, 100f, t), 0f);

            yield return null;
        }

        ResetShimmer();
    }

    // ── Reset All ────────────────────────────────────────

    public void ResetAll()
    {
        // ── FIX: Set isPlayable = false FIRST so PulseGlow()
        //    exits its while loop immediately when it next checks.
        //    Then stop coroutines to cancel the wait/delay.
        isPlayable = false;

        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
            glowCoroutine = null;
        }

        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }

        if (shimmerCoroutine != null)
        {
            StopCoroutine(shimmerCoroutine);
            shimmerCoroutine = null;
        }

        ResetGlow();
        ResetShimmer();
    }
}