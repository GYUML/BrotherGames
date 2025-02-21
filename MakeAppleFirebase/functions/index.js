const {onRequest} = require("firebase-functions/v2/https");
const {initializeApp} = require("firebase-admin/app");
const {getFirestore} = require("firebase-admin/firestore");

initializeApp();

const NumberOfTop = 100;

//  Take the text parameter passed to this HTTP endpoint and insert it into
//  Firestore under the path /messages/:documentId/original
exports.addScore = onRequest(async (req, res) => {
  try {
    // Grab the text parameter.
    const uid = req.query.uid;
    const score = Number(req.query.score);

    const db = getFirestore();
    const userRef = db.collection("UserInfo").doc(uid);
    const userData = await userRef.get();
    const today = new Date();
    let isBest = false;
    if (!userData.exists) {
      isBest = true;
    } else {
      if (userData.data().bestScore < score) isBest = true;
    }

    if (isBest == true) {
      await userRef.set({
        bestScore: score,
        today: today,
      }, {merge: true});
      await checkRanking(uid, score);
    }

    res.json({
      result: {
        score: score,
        isBest: isBest,
      },
    });
  } catch (error) {
    console.log(error);
  }
});


exports.getRankings = onRequest(async (req, res) => {
  const db = getFirestore();
  const topUserRef = db.collection("TopRanking").doc("TopUsers");
  const topUsers = await topUserRef.get();
  if (topUsers.exists) {
    const arr = topUsers.data().topUsers;
    res.json(arr.slice(0, NumberOfTop));
  } else {
    console.log("topUsers not found");
  }

  return null;
});

// forceRanking();

/**
 * @async forceRanking
 */
async function forceRanking() {
  try {
    const db = getFirestore();
    const topUserRef = db.collection("TopRanking").doc("TopUsers");
    const snapshot = await db.collection("UserInfo")
        .orderBy("bestScore", "desc")
        .limit(NumberOfTop)
        .get();

    const topUsers = [];
    snapshot.forEach((doc) => {
      topUsers.push({
        name: doc.id,
        score: doc.data().bestScore,
      });
    });

    await topUserRef.set({
      topUsers,
    }, {merge: true});
    // res.json(topUsers);
  } catch (error) {
    console.log(error);
  }

  return null;
}


/**
 * @param {string} uid
 * @param {number} score
 */
async function checkRanking(uid, score) {
  const db = getFirestore();
  const topUserRef = db.collection("TopRanking").doc("TopUsers");
  const topUsers = await topUserRef.get();
  if (topUsers.exists) {
    const arr = topUsers.data().topUsers;
    const resArr = await sortRanking(arr, uid, score);
    await topUserRef.set({
      topUsers: resArr.slice(0, NumberOfTop),
    }, {merge: true});
  } else {
    console.log("topUsers not found");
  }
}


/**
 * @param {Array} arr
 * @param {string} uid
 * @param {number} newScore
 */
async function sortRanking(arr, uid, newScore) {
  let i = arr.length;
  while (0 < i && arr[i - 1].score < newScore) {
    if (arr[i - 1].name == uid) arr.splice(i-1, 1); // Top10에 이미 ID가 있다면 삭제
    i--;
  }
  arr.splice(i, 0, {
    name: uid,
    score: newScore,
  });
  return arr;
}
