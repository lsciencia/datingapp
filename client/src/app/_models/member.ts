import { Photo } from "./photo";

export interface Member {
    id: number;
    username: string;
    gender: string;
    age: number;
    photoUrl: string;
    dateOfBirth: Date;
    knownAs: string;
    created: Date;
    lastActive: Date;
    introduction: string;
    lookingFor: string;
    interests: string;
    city: string;
    country: string;
    photos: Photo[];
}


