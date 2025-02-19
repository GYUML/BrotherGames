/**
 * Import function triggers from their respective submodules:
 *
 * const {onCall} = require("firebase-functions/v2/https");
 * const {onDocumentWritten} = require("firebase-functions/v2/firestore");
 *
 * See a full list of supported triggers at https://firebase.google.com/docs/functions
 */

// The Cloud Functions for Firebase SDK to create Cloud Functions and triggers.
// const {logger} = require("firebase-functions");
const {onRequest} = require("firebase-functions/v2/https");
// const {
//   onDocumentWritten,
//   onDocumentCreated,
//   onDocumentUpdated,
//   onDocumentDeleted,
//   Change,
//   FirestoreEvent
// } = require("firebase-functions/v2/firestore");

// The Firebase Admin SDK to access Firestore.
const {initializeApp} = require("firebase-admin/app");
const {getFirestore} = require("firebase-admin/firestore");

initializeApp();

const NumberOfTop = 3;

//  Take the text parameter passed to this HTTP endpoint and insert it into
//  Firestore under the path /messages/:documentId/original
exports.saveScore = onRequest(async (req, res) => {
  try {
    // Grab the text parameter.
    const uid = req.query.uid;
    const score = Number(req.query.score);

    const db = getFirestore();
    const userRef = db.collection("Ranking").doc(uid);
    let isBest = false;
    const userData = await userRef.get();
    const today = new Date();
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
  try {
    const db = getFirestore();
    const snapshot = await db.collection("Ranking")
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
    res.json(topUsers);
  } catch (error) {
    console.log(error);
  }

  return null;
});


exports.getTopRanking = onRequest(async (req, res) => {
  try {
    const db = getFirestore();
    const topUserRef = db.collection("TopRanking").doc("topUsers");
    const snapshot = await db.collection("Ranking")
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
      topUsers
    }, {merge: true});
    res.json(topUsers);
  } catch (error) {
    console.log(error);
  }

  return null;
});


async function checkRanking(uid, score){
  const db = getFirestore();
  const topUserRef = db.collection("TopRanking").doc("topUsers");
  const topUsers = await topUserRef.get();
  if(topUsers.exists){
    const arr = topUsers.data().topUsers;
    const resArr = await updateRanking(arr, uid, score);
    console.log(resArr);
    await topUserRef.set({
      topUsers: resArr
    }, {merge: true});
    // 자기가 이미 Top10인데 갱신했다면 기존 Top10에서 자기 이름은 빼야함
    // 따라서 Top 랭킹 출력은 10명만 하더라도 데이터는 11명 가지고 있어야함.
    // resArr의 크기 제한은 없으므로 11명으로 제한하는 코드 필요
  } else{
    //getTopRanking()  처음에는 topUsers 문서가 없으므로 getTopRanking 실행 필요
    console.log("topUsers not found");
  }
  
}


async function updateRanking(arr, uid, newScore){
  
  let i = arr.length;
  while(0 < i && arr[i - 1].score < newScore){
    if(arr[i - 1].name == uid) arr.splice(i-1, 1);  // Top10에 이미 ID가 있다면 삭제
    i--;
  }
  arr.splice(i,0,{
    name: uid,
    score: newScore
  });
  return arr;
}
