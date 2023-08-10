import jwt from "jsonwebtoken";
import { TokenUser, UserVO } from "./Types";
import { JWT_SECRET } from "./Secret";
import { Request, Response, NextFunction } from "express";

export const tokenChecker = async (req :Request, res :Response, next :NextFunction) => {
    let token = extractToken(req);
    if(token != undefined){
        let decodedToken = decodeJWT(token) as TokenUser;
        if(decodedToken != null){
            let {id, email, xp, name} = decodeJWT(token) as TokenUser;
            req.user = { id, email, exp: xp, name };
        }
        else{
            req.user = null;
        }
    }
    else{
        req.user = null;
    }

    next();
};

function extractToken(req :Request){
    const PREFIX = "Bearer";
    const auth = req.headers.authorization;
    // PREFIX를 포함한 토큰이면 PREFIX 제거, 아니라면 그냥 토큰을 넘기면 됨
    const token = auth?.includes(PREFIX) ? auth.split(PREFIX)[1] : auth;
    // token이 존재하지 않으면 undifineded가 리턴될거임
    return token;
}

export const createJWT = (userVo :UserVO) => {
    const { id, email, name, exp } = userVo;

    // 10h, 100s, 50m 등 가능. 정확하게 하고 싶으면 ms단위로 그냥 넣어도 됨
    const token = jwt.sign({ id, email, name, xp: exp }, JWT_SECRET!, {expiresIn: "7 days"});
    
    return token;
};

export const decodeJWT = (token :string) => {
    try{
        // 넘어온 토큰을 디코드 -> 시크릿 키가 다르면 해독은 되도 무용지물임
        const decodedToken = jwt.verify(token, JWT_SECRET);
        return decodedToken;
    }
    catch(e){
        return null;
    }
};