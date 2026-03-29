using UnityEngine;

public class ColorChangerCommand
{
    private SpriteRenderer spriteRenderer;

    public ColorChangerCommand(SpriteRenderer renderer)
    {
        spriteRenderer = renderer;
    }

    public void Execute()
    {
        spriteRenderer.color = new Color(Random.value, Random.value, Random.value);
    }
}
