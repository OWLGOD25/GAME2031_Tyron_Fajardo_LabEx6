using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerTests
{
    private GameObject player;

    [SetUp]
    public void Setup()
    {
        player = new GameObject("Player");

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;

        player.AddComponent<BoxCollider2D>();
        player.AddComponent<Player2>();
    }

    [Test]
    public void Player_HasRequiredComponents()
    {
        Assert.IsNotNull(player.GetComponent<Rigidbody2D>());
        Assert.IsNotNull(player.GetComponent<Collider2D>());
        Assert.IsNotNull(player.GetComponent<Player2>());
    }

    [Test]
    public void Rigidbody_IsConfiguredCorrectly()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        Assert.AreEqual(RigidbodyType2D.Dynamic, rb.bodyType);
        Assert.AreEqual(0, rb.gravityScale);
    }
}
