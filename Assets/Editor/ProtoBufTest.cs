using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using gameplay;
using System.IO;

public class ProtoBufTest {

	[Test]
	public void ProtoBufTestSimplePasses() {
        // Use the Assert class to test conditions.
        //Arrange
        HeartBeat inbeat = new HeartBeat();
        inbeat.TimeStamp = 1.989f;

        //Act
        var ser = proto.ProtobufSerializer.Create();
        MemoryStream stream = new MemoryStream();
        ser.Serialize(stream, inbeat);

        stream.Seek(0, SeekOrigin.Begin);

        HeartBeat outbeat = null;
        outbeat = ser.Deserialize(stream, outbeat, typeof(HeartBeat)) as HeartBeat;

        //Assert
        Assert.AreEqual(inbeat.TimeStamp, outbeat.TimeStamp);
    }

    /*
	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator ProtoBufTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame

		yield return null;
	}*/
}
