﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BigSchool.Models;
using Microsoft.AspNet.Identity;

namespace BigSchool.Controllers
{
    public class FollowingsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Follow(Following follow)
        {
            //user login là người theo dõi, follow.FolloweeId là người được theo dõi

            var userID = User.Identity.GetUserId();
            if (userID == null)
                return BadRequest("Please login first!");
            if (userID == follow.FolloweeID)
                return BadRequest("Can not follow myself!");
            BigSchoolContext context = new BigSchoolContext();
            //kiểm tra xem mã userID đã được theo dõi chưa

            Following find = context.Followings.FirstOrDefault(p => p.FollowerID ==
            userID && p.FolloweeID == follow.FolloweeID);
            if (find != null)

            {
                context.Followings.Remove(context.Followings.SingleOrDefault(p =>
                                            p.FollowerID == userID && p.FolloweeID == follow.FolloweeID));
                context.SaveChanges();
                return Ok("cancel");

            }
            //set object follow
            follow.FollowerID = userID;
            context.Followings.Add(follow);
            context.SaveChanges();
            return Ok();
        }
    }
}