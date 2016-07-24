# Social Feed Domain Notes

## Structure

* Posts
    * Comments
    * Likes

## Queries

* Get latest 10 posts from friendship graph
    * order by timestamp
    * need to know if more exist
    * perhaps return the proper URL for the next call

* Commands
    * Add new post
        * Treat all as aync
        * Return `CREATE_JOB`
        * When status is `PENDING`, client can poll `statusUrl`
        * When task is sync, perhaps skip job record creation
    * Comment on post
    * Toggle post kudos

```
type CREATE_JOB = {
    type: string; // EX: Post
    status: 'COMPLETE' | 'PENDING' | 'FAILED';
    statusUrl: string; // url returns CREATE_JOB
    contentUrl: string;
}
```

## Domain Details

```
type POST = {
    id: number;
    userId: number;
    content: Array<POST_CONTENT>;
    kudos: Array<POST_KUDOS>;
    comments: Array<POST_COMMENT>;
    timestamp: date;
}

type POST_CONTENT = {
    id: number;
    postId: number;
    type: POST_CONTENT_TYPE;
    value: string;
    meta: Object;
}

type POST_CONTENT_TYPE = 'text' | 'picture'

-------------------------------------------------

type POST_COMMENT = {
    id: number;
    postId: number;
    userId: number;
    value: string;
    timestamp: date;
}

type POST_KUDOS = {
    postId: number;
    userId: number;
}
```
