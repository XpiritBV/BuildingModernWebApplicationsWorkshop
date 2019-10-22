/* tslint:disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';



/**
 * API to retrieve or post individual high scores
 */
@Injectable({
  providedIn: 'root',
})
export class ScoresService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation scoresPostScore
   */
  static readonly ScoresPostScorePath = '/api/v1.0/Scores/{nickname}/{game}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `scoresPostScore()` instead.
   *
   * This method sends `application/json` and handles response body of type `application/json`
   */
  scoresPostScore$Response(params: {
    nickname: null | string;
    game: null | string;
    version: string;

    body: number
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ScoresService.ScoresPostScorePath, 'post');
    if (params) {

      rb.path('nickname', params.nickname);
      rb.path('game', params.game);
      rb.path('version', params.version);

      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `scoresPostScore$Response()` instead.
   *
   * This method sends `application/json` and handles response body of type `application/json`
   */
  scoresPostScore(params: {
    nickname: null | string;
    game: null | string;
    version: string;

    body: number
  }): Observable<void> {

    return this.scoresPostScore$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
