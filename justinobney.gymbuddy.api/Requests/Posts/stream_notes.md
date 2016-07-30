# stream notes

## timeline types

* standard to all
    * userName
    * profilePictureUrl
    * timestamp
    * kudosCount
    * comments
* text
    * text
* image
    * text
    * imageUrl
* workout
    * muscleGroup
    * scheduleDate
    * guests `[ ]`
        * userName
        * profilePictureUrl

## streams

* user_posts (flat)
* user (flat)
* timeline (flat)
* notification (notification)

## activity

* user_posts
    * gets "timeline activities"
* user (flat) (TODO: later)
    * gets notifications via "to"

## follows

* on profile create: `timeline:a` => `user_posts:a`
* on confirm friendship: `timeline:a` => `user_posts:b`

## actions

* create post
* delete post
* toggle kudos
* add comment to post
* plan workout
* update workout time
* comment on workout
* cancel workout
