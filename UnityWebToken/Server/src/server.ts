import express, { Application, Request, Response } from "express";
import nunjucks from "nunjucks";

// 라우터 가져오기
import { lunchRouter } from "./LunchRouter";
import { userRouter } from "./UserRouter";
import { tokenChecker } from "./MyJWT";

// 익스프레스 어플리케이션 = 웹서버!
let app :Application = express();

app.set("view engine", "njk");
nunjucks.configure("views", {express:app, watch:true});

// POST로 들어오는 데어터들을 json형태로 파싱해주겠다.
app.use(express.json());
app.use(express.urlencoded({ extended:true }));

// 토큰 체크
app.use(tokenChecker);

// Get, Post, Put, Delete => Method
// CRUD => Create, Read, Update, Delete
// Application단계에서 CRUD구현 => API
// URI | Get(Read), Post(Create), Put(Update), Delete(Delete)
// RestFul API

// 점심관련 라우터
app.use(lunchRouter);
// 유저관련 라우터
app.use(userRouter);

app.listen(3000, () => {
    console.log(
        `
        ######################################
        #  Server is starting on 3000 port   #
        #  http://localhost:3000             #
        ######################################
        `
    );
});