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


//  Take the text parameter passed to this HTTP endpoint and insert it into
//  Firestore under the path /messages/:documentId/original
exports.addmessage = onRequest(async (req, res) => {
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
        .limit(3)
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
